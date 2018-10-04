using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Cryptography;
using TrustEDU.Core.Models.Coins;
using TrustEDU.Core.Models.Inventory;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.Network;
using TrustEDU.Core.Models.SmartContract;
using TrustEDU.Core.Models.Transactions;
using TrustEDU.Core.Models.Wallets;
using TrustEDU.Core.Network.Peer2Peer;
using TrustEDU.Core.Plugins;

namespace TrustEDU.Core.Consensus
{
    public sealed class ConsensusService : UntypedActor
    {
        public class Start { }
        internal class Timer { public uint Height; public byte ViewNumber; }

        private readonly ConsensusContext _context = new ConsensusContext();
        private readonly TrustEDUNetwork _system;
        private readonly Wallet _wallet;
        private DateTime _blockReceivedTime;

        public ConsensusService(TrustEDUNetwork system, Wallet wallet)
        {
            this._system = system;
            this._wallet = wallet;
        }

        private bool AddTransaction(Transaction tx, bool verify)
        {
            if (_context.Snapshot.ContainsTransaction(tx.Hash) ||
                (verify && !tx.Verify(_context.Snapshot, _context.Transactions.Values)) ||
                !Plugin.CheckPolicy(tx))
            {
                Log($"reject tx: {tx.Hash}{Environment.NewLine}{tx.ToArray().ToHexString()}", LogLevel.Warning);
                RequestChangeView();
                return false;
            }
            _context.Transactions[tx.Hash] = tx;
            if (_context.TransactionHashes.Length == _context.Transactions.Count)
            {
                if (Blockchain.GetConsensusAddress(_context.Snapshot.GetValidators(_context.Transactions.Values).ToArray()).Equals(_context.NextConsensus))
                {
                    Log($"send prepare response");
                    _context.State |= ConsensusState.SignatureSent;
                    _context.Signatures[_context.MyIndex] = _context.MakeHeader().Sign(_context.KeyPair);
                    SignAndRelay(_context.MakePrepareResponse(_context.Signatures[_context.MyIndex]));
                    CheckSignatures();
                }
                else
                {
                    RequestChangeView();
                    return false;
                }
            }
            return true;
        }

        private void ChangeTimer(TimeSpan delay)
        {
            Context.System.Scheduler.ScheduleTellOnce(delay, Self, new Timer
            {
                Height = _context.BlockIndex,
                ViewNumber = _context.ViewNumber
            }, ActorRefs.NoSender);
        }

        private void CheckExpectedView(byte viewNumber)
        {
            if (_context.ViewNumber == viewNumber) return;
            if (_context.ExpectedView.Count(p => p == viewNumber) >= _context.M)
            {
                InitializeConsensus(viewNumber);
            }
        }

        private void CheckSignatures()
        {
            if (_context.Signatures.Count(p => p != null) >= _context.M && _context.TransactionHashes.All(p => _context.Transactions.ContainsKey(p)))
            {
                Contract contract = Contract.CreateMultiSigContract(_context.M, _context.Validators);
                Block block = _context.MakeHeader();
                ContractParametersContext sc = new ContractParametersContext(block);
                for (int i = 0, j = 0; i < _context.Validators.Length && j < _context.M; i++)
                    if (_context.Signatures[i] != null)
                    {
                        sc.AddSignature(contract, _context.Validators[i], _context.Signatures[i]);
                        j++;
                    }
                sc.Verifiable.Witnesses = sc.GetWitnesses();
                block.Transactions = _context.TransactionHashes.Select(p => _context.Transactions[p]).ToArray();
                Log($"relay block: {block.Hash}");
                _system.LocalNode.Tell(new LocalNode.Relay { Inventory = block });
                _context.State |= ConsensusState.BlockSent;
            }
        }

        private void FillContext()
        {
            IEnumerable<Transaction> memPool = Blockchain.Singleton.GetMemoryPool();
            foreach (IPolicyPlugin plugin in Plugin.Policies)
                memPool = plugin.FilterForBlock(memPool);
            List<Transaction> transactions = memPool.ToList();
            Fixed8 amountNetfee = Block.CalculateNetFee(transactions);
            TransactionOutput[] outputs = amountNetfee == Fixed8.Zero ? new TransactionOutput[0] : new[] { new TransactionOutput
            {
                AssetId = Blockchain.UtilityToken.Hash,
                Value = amountNetfee,
                ScriptHash = _wallet.GetChangeAddress()
            } };
            while (true)
            {
                ulong nonce = GetNonce();
                MinerTransaction tx = new MinerTransaction
                {
                    Nonce = (uint)(nonce % (uint.MaxValue + 1ul)),
                    Attributes = new TransactionAttribute[0],
                    Inputs = new CoinReference[0],
                    Outputs = outputs,
                    Witnesses = new Witness[0]
                };
                if (!_context.Snapshot.ContainsTransaction(tx.Hash))
                {
                    _context.Nonce = nonce;
                    transactions.Insert(0, tx);
                    break;
                }
            }
            _context.TransactionHashes = transactions.Select(p => p.Hash).ToArray();
            _context.Transactions = transactions.ToDictionary(p => p.Hash);
            _context.NextConsensus = Blockchain.GetConsensusAddress(_context.Snapshot.GetValidators(transactions).ToArray());
        }

        private static ulong GetNonce()
        {
            byte[] nonce = new byte[sizeof(ulong)];
            Random rand = new Random();
            rand.NextBytes(nonce);
            return nonce.ToUInt64(0);
        }

        private void InitializeConsensus(byte viewNumber)
        {
            if (viewNumber == 0)
                _context.Reset(_wallet);
            else
                _context.ChangeView(viewNumber);
            if (_context.MyIndex < 0) return;
            if (viewNumber > 0)
                Log($"changeview: view={viewNumber} primary={_context.Validators[_context.GetPrimaryIndex((byte)(viewNumber - 1u))]}", LogLevel.Warning);
            Log($"initialize: height={_context.BlockIndex} view={viewNumber} index={_context.MyIndex} role={(_context.MyIndex == _context.PrimaryIndex ? ConsensusState.Primary : ConsensusState.Backup)}");
            if (_context.MyIndex == _context.PrimaryIndex)
            {
                _context.State |= ConsensusState.Primary;
                TimeSpan span = DateTime.Now - _blockReceivedTime;
                if (span >= Blockchain.TimePerBlock)
                    ChangeTimer(TimeSpan.Zero);
                else
                    ChangeTimer(Blockchain.TimePerBlock - span);
            }
            else
            {
                _context.State = ConsensusState.Backup;
                ChangeTimer(TimeSpan.FromSeconds(Blockchain.SecondsPerBlock << (viewNumber + 1)));
            }
        }

        private void Log(string message, LogLevel level = LogLevel.Info)
        {
            Plugin.Log(nameof(ConsensusService), level, message);
        }

        private void OnChangeViewReceived(ConsensusPayload payload, ChangeView message)
        {
            Log($"{nameof(OnChangeViewReceived)}: height={payload.BlockIndex} view={message.ViewNumber} index={payload.ValidatorIndex} nv={message.NewViewNumber}");
            if (message.NewViewNumber <= _context.ExpectedView[payload.ValidatorIndex])
                return;
            _context.ExpectedView[payload.ValidatorIndex] = message.NewViewNumber;
            CheckExpectedView(message.NewViewNumber);
        }

        private void OnConsensusPayload(ConsensusPayload payload)
        {
            if (payload.ValidatorIndex == _context.MyIndex) return;
            if (payload.Version != ConsensusContext.Version)
                return;
            if (payload.PrevHash != _context.PrevHash || payload.BlockIndex != _context.BlockIndex)
            {
                if (_context.Snapshot.Height + 1 < payload.BlockIndex)
                {
                    Log($"chain sync: expected={payload.BlockIndex} current: {_context.Snapshot.Height} nodes={LocalNode.Singleton.ConnectedCount}", LogLevel.Warning);
                }
                return;
            }
            if (payload.ValidatorIndex >= _context.Validators.Length) return;
            ConsensusMessage message;
            try
            {
                message = ConsensusMessage.DeserializeFrom(payload.Data);
            }
            catch
            {
                return;
            }
            if (message.ViewNumber != _context.ViewNumber && message.Type != ConsensusMessageType.ChangeView)
                return;
            switch (message.Type)
            {
                case ConsensusMessageType.ChangeView:
                    OnChangeViewReceived(payload, (ChangeView)message);
                    break;
                case ConsensusMessageType.PrepareRequest:
                    OnPrepareRequestReceived(payload, (PrepareRequest)message);
                    break;
                case ConsensusMessageType.PrepareResponse:
                    OnPrepareResponseReceived(payload, (PrepareResponse)message);
                    break;
            }
        }

        private void OnPersistCompleted(Block block)
        {
            Log($"persist block: {block.Hash}");
            _blockReceivedTime = DateTime.Now;
            InitializeConsensus(0);
        }

        private void OnPrepareRequestReceived(ConsensusPayload payload, PrepareRequest message)
        {
            Log($"{nameof(OnPrepareRequestReceived)}: height={payload.BlockIndex} view={message.ViewNumber} index={payload.ValidatorIndex} tx={message.TransactionHashes.Length}");
            if (!_context.State.HasFlag(ConsensusState.Backup) || _context.State.HasFlag(ConsensusState.RequestReceived))
                return;
            if (payload.ValidatorIndex != _context.PrimaryIndex) return;
            if (payload.Timestamp <= _context.Snapshot.GetHeader(_context.PrevHash).Timestamp || payload.Timestamp > DateTime.Now.AddMinutes(10).ToTimestamp())
            {
                Log($"Timestamp incorrect: {payload.Timestamp}", LogLevel.Warning);
                return;
            }
            _context.State |= ConsensusState.RequestReceived;
            _context.Timestamp = payload.Timestamp;
            _context.Nonce = message.Nonce;
            _context.NextConsensus = message.NextConsensus;
            _context.TransactionHashes = message.TransactionHashes;
            _context.Transactions = new Dictionary<UInt256, Transaction>();
            if (!Crypto.Default.VerifySignature(_context.MakeHeader().GetHashData(), message.Signature, _context.Validators[payload.ValidatorIndex].EncodePoint(false))) return;
            _context.Signatures = new byte[_context.Validators.Length][];
            _context.Signatures[payload.ValidatorIndex] = message.Signature;
            Dictionary<UInt256, Transaction> mempool = Blockchain.Singleton.GetMemoryPool().ToDictionary(p => p.Hash);
            foreach (UInt256 hash in _context.TransactionHashes.Skip(1))
            {
                if (mempool.TryGetValue(hash, out Transaction tx))
                    if (!AddTransaction(tx, false))
                        return;
            }
            if (!AddTransaction(message.MinerTransaction, true)) return;
            if (_context.Transactions.Count < _context.TransactionHashes.Length)
            {
                UInt256[] hashes = _context.TransactionHashes.Where(i => !_context.Transactions.ContainsKey(i)).ToArray();
                _system.TaskManager.Tell(new TaskManager.RestartTasks
                {
                    Payload = InvPayload.Create(InventoryType.Tx, hashes)
                });
            }
        }

        private void OnPrepareResponseReceived(ConsensusPayload payload, PrepareResponse message)
        {
            Log($"{nameof(OnPrepareResponseReceived)}: height={payload.BlockIndex} view={message.ViewNumber} index={payload.ValidatorIndex}");
            if (_context.State.HasFlag(ConsensusState.BlockSent)) return;
            if (_context.Signatures[payload.ValidatorIndex] != null) return;
            Block header = _context.MakeHeader();
            if (header == null || !Crypto.Default.VerifySignature(header.GetHashData(), message.Signature, _context.Validators[payload.ValidatorIndex].EncodePoint(false))) return;
            _context.Signatures[payload.ValidatorIndex] = message.Signature;
            CheckSignatures();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Start _:
                    OnStart();
                    break;
                case Timer timer:
                    OnTimer(timer);
                    break;
                case ConsensusPayload payload:
                    OnConsensusPayload(payload);
                    break;
                case Transaction transaction:
                    OnTransaction(transaction);
                    break;
                case Blockchain.PersistCompleted completed:
                    OnPersistCompleted(completed.Block);
                    break;
            }
        }

        private void OnStart()
        {
            Log("OnStart");
            InitializeConsensus(0);
        }

        private void OnTimer(Timer timer)
        {
            if (timer.Height != _context.BlockIndex || timer.ViewNumber != _context.ViewNumber) return;
            Log($"timeout: height={timer.Height} view={timer.ViewNumber} state={_context.State}");
            if (_context.State.HasFlag(ConsensusState.Primary) && !_context.State.HasFlag(ConsensusState.RequestSent))
            {
                Log($"send prepare request: height={timer.Height} view={timer.ViewNumber}");
                _context.State |= ConsensusState.RequestSent;
                if (!_context.State.HasFlag(ConsensusState.SignatureSent))
                {
                    FillContext();
                    _context.Timestamp = Math.Max(DateTime.Now.ToTimestamp(), _context.Snapshot.GetHeader(_context.PrevHash).Timestamp + 1);
                    _context.Signatures[_context.MyIndex] = _context.MakeHeader().Sign(_context.KeyPair);
                }
                SignAndRelay(_context.MakePrepareRequest());
                if (_context.TransactionHashes.Length > 1)
                {
                    foreach (InvPayload payload in InvPayload.CreateGroup(InventoryType.Tx, _context.TransactionHashes.Skip(1).ToArray()))
                        _system.LocalNode.Tell(Message.Create("inv", payload));
                }
                ChangeTimer(TimeSpan.FromSeconds(Blockchain.SecondsPerBlock << (timer.ViewNumber + 1)));
            }
            else if ((_context.State.HasFlag(ConsensusState.Primary) && _context.State.HasFlag(ConsensusState.RequestSent)) || _context.State.HasFlag(ConsensusState.Backup))
            {
                RequestChangeView();
            }
        }

        private void OnTransaction(Transaction transaction)
        {
            if (transaction.Type == TransactionType.MinerTransaction) return;
            if (!_context.State.HasFlag(ConsensusState.Backup) || !_context.State.HasFlag(ConsensusState.RequestReceived) || _context.State.HasFlag(ConsensusState.SignatureSent) || _context.State.HasFlag(ConsensusState.ViewChanging))
                return;
            if (_context.Transactions.ContainsKey(transaction.Hash)) return;
            if (!_context.TransactionHashes.Contains(transaction.Hash)) return;
            AddTransaction(transaction, true);
        }

        protected override void PostStop()
        {
            Log("OnStop");
            _context.Dispose();
            base.PostStop();
        }

        public static Props Props(TrustEDUNetwork system, Wallet wallet)
        {
            return Akka.Actor.Props.Create(() => new ConsensusService(system, wallet)).WithMailbox("consensus-service-mailbox");
        }

        private void RequestChangeView()
        {
            _context.State |= ConsensusState.ViewChanging;
            _context.ExpectedView[_context.MyIndex]++;
            Log($"request change view: height={_context.BlockIndex} view={_context.ViewNumber} nv={_context.ExpectedView[_context.MyIndex]} state={_context.State}");
            ChangeTimer(TimeSpan.FromSeconds(Blockchain.SecondsPerBlock << (_context.ExpectedView[_context.MyIndex] + 1)));
            SignAndRelay(_context.MakeChangeView());
            CheckExpectedView(_context.ExpectedView[_context.MyIndex]);
        }

        private void SignAndRelay(ConsensusPayload payload)
        {
            ContractParametersContext sc;
            try
            {
                sc = new ContractParametersContext(payload);
                _wallet.Sign(sc);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            sc.Verifiable.Witnesses = sc.GetWitnesses();
            _system.LocalNode.Tell(new LocalNode.SendDirectly { Inventory = payload });
        }
    }
}
