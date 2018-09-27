using System;
using System.IO;
using System.Linq;
using System.Text;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Cryptography.ECC;
using TrustEDU.Core.Models.Assets;
using TrustEDU.Core.Models.Coins;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.Services;
using TrustEDU.Core.Models.SmartContract.Enumerators;
using TrustEDU.Core.Models.Transactions;
using TrustEDU.Core.Models.Wallets;
using TrustEDU.VM.Base;
using TrustEDU.VM.Base.Types;
using TrustEDU.VM.Runtime;
using Snapshot = TrustEDU.Core.Persistence.Snapshot;
using VMArray = TrustEDU.VM.Base.Types.Array;

namespace TrustEDU.Core.Models.SmartContract
{
    public class TrustEDUService: StandardService
    {
        public TrustEDUService(TriggerType trigger, Snapshot snapshot)
            : base(trigger, snapshot)
        {
            Register("TrustEDU.Runtime.GetTrigger", Runtime_GetTrigger);
            Register("TrustEDU.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("TrustEDU.Runtime.Notify", Runtime_Notify);
            Register("TrustEDU.Runtime.Log", Runtime_Log);
            Register("TrustEDU.Runtime.GetTime", Runtime_GetTime);
            Register("TrustEDU.Runtime.Serialize", Runtime_Serialize);
            Register("TrustEDU.Runtime.Deserialize", Runtime_Deserialize);
            Register("TrustEDU.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("TrustEDU.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("TrustEDU.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("TrustEDU.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("TrustEDU.Blockchain.GetTransactionHeight", Blockchain_GetTransactionHeight);
            Register("TrustEDU.Blockchain.GetAccount", Blockchain_GetAccount);
            Register("TrustEDU.Blockchain.GetValidators", Blockchain_GetValidators);
            Register("TrustEDU.Blockchain.GetAsset", Blockchain_GetAsset);
            Register("TrustEDU.Blockchain.GetContract", Blockchain_GetContract);
            Register("TrustEDU.Header.GetHash", Header_GetHash);
            Register("TrustEDU.Header.GetVersion", Header_GetVersion);
            Register("TrustEDU.Header.GetPrevHash", Header_GetPrevHash);
            Register("TrustEDU.Header.GetMerkleRoot", Header_GetMerkleRoot);
            Register("TrustEDU.Header.GetTimestamp", Header_GetTimestamp);
            Register("TrustEDU.Header.GetIndex", Header_GetIndex);
            Register("TrustEDU.Header.GetConsensusData", Header_GetConsensusData);
            Register("TrustEDU.Header.GetNextConsensus", Header_GetNextConsensus);
            Register("TrustEDU.Block.GetTransactionCount", Block_GetTransactionCount);
            Register("TrustEDU.Block.GetTransactions", Block_GetTransactions);
            Register("TrustEDU.Block.GetTransaction", Block_GetTransaction);
            Register("TrustEDU.Transaction.GetHash", Transaction_GetHash);
            Register("TrustEDU.Transaction.GetType", Transaction_GetType);
            Register("TrustEDU.Transaction.GetAttributes", Transaction_GetAttributes);
            Register("TrustEDU.Transaction.GetInputs", Transaction_GetInputs);
            Register("TrustEDU.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("TrustEDU.Transaction.GetReferences", Transaction_GetReferences);
            Register("TrustEDU.Transaction.GetUnspentCoins", Transaction_GetUnspentCoins);
            Register("TrustEDU.InvocationTransaction.GetScript", InvocationTransaction_GetScript);
            Register("TrustEDU.Attribute.GetUsage", Attribute_GetUsage);
            Register("TrustEDU.Attribute.GetData", Attribute_GetData);
            Register("TrustEDU.Input.GetHash", Input_GetHash);
            Register("TrustEDU.Input.GetIndex", Input_GetIndex);
            Register("TrustEDU.Output.GetAssetId", Output_GetAssetId);
            Register("TrustEDU.Output.GetValue", Output_GetValue);
            Register("TrustEDU.Output.GetScriptHash", Output_GetScriptHash);
            Register("TrustEDU.Account.GetScriptHash", Account_GetScriptHash);
            Register("TrustEDU.Account.GetVotes", Account_GetVotes);
            Register("TrustEDU.Account.GetBalance", Account_GetBalance);
            Register("TrustEDU.Asset.Create", Asset_Create);
            Register("TrustEDU.Asset.Renew", Asset_Renew);
            Register("TrustEDU.Asset.GetAssetId", Asset_GetAssetId);
            Register("TrustEDU.Asset.GetAssetType", Asset_GetAssetType);
            Register("TrustEDU.Asset.GetAmount", Asset_GetAmount);
            Register("TrustEDU.Asset.GetAvailable", Asset_GetAvailable);
            Register("TrustEDU.Asset.GetPrecision", Asset_GetPrecision);
            Register("TrustEDU.Asset.GetOwner", Asset_GetOwner);
            Register("TrustEDU.Asset.GetAdmin", Asset_GetAdmin);
            Register("TrustEDU.Asset.GetIssuer", Asset_GetIssuer);
            Register("TrustEDU.Contract.Create", Contract_Create);
            Register("TrustEDU.Contract.Migrate", Contract_Migrate);
            Register("TrustEDU.Contract.Destroy", Contract_Destroy);
            Register("TrustEDU.Contract.GetScript", Contract_GetScript);
            Register("TrustEDU.Contract.IsPayable", Contract_IsPayable);
            Register("TrustEDU.Contract.GetStorageContext", Contract_GetStorageContext);
            Register("TrustEDU.Storage.GetContext", Storage_GetContext);
            Register("TrustEDU.Storage.GetReadOnlyContext", Storage_GetReadOnlyContext);
            Register("TrustEDU.Storage.Get", Storage_Get);
            Register("TrustEDU.Storage.Put", Storage_Put);
            Register("TrustEDU.Storage.Delete", Storage_Delete);
            Register("TrustEDU.Storage.Find", Storage_Find);
            Register("TrustEDU.StorageContext.AsReadOnly", StorageContext_AsReadOnly);
            Register("TrustEDU.Enumerator.Create", Enumerator_Create);
            Register("TrustEDU.Enumerator.Next", Enumerator_Next);
            Register("TrustEDU.Enumerator.Value", Enumerator_Value);
            Register("TrustEDU.Enumerator.Concat", Enumerator_Concat);
            Register("TrustEDU.Iterator.Create", Iterator_Create);
            Register("TrustEDU.Iterator.Key", Iterator_Key);
            Register("TrustEDU.Iterator.Keys", Iterator_Keys);
            Register("TrustEDU.Iterator.Values", Iterator_Values);

            Register("TrustEDU.Iterator.Next", Enumerator_Next);
            Register("TrustEDU.Iterator.Value", Enumerator_Value);

            Register("AntShares.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("AntShares.Runtime.Notify", Runtime_Notify);
            Register("AntShares.Runtime.Log", Runtime_Log);
            Register("AntShares.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("AntShares.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("AntShares.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("AntShares.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("AntShares.Blockchain.GetAccount", Blockchain_GetAccount);
            Register("AntShares.Blockchain.GetValidators", Blockchain_GetValidators);
            Register("AntShares.Blockchain.GetAsset", Blockchain_GetAsset);
            Register("AntShares.Blockchain.GetContract", Blockchain_GetContract);
            Register("AntShares.Header.GetHash", Header_GetHash);
            Register("AntShares.Header.GetVersion", Header_GetVersion);
            Register("AntShares.Header.GetPrevHash", Header_GetPrevHash);
            Register("AntShares.Header.GetMerkleRoot", Header_GetMerkleRoot);
            Register("AntShares.Header.GetTimestamp", Header_GetTimestamp);
            Register("AntShares.Header.GetConsensusData", Header_GetConsensusData);
            Register("AntShares.Header.GetNextConsensus", Header_GetNextConsensus);
            Register("AntShares.Block.GetTransactionCount", Block_GetTransactionCount);
            Register("AntShares.Block.GetTransactions", Block_GetTransactions);
            Register("AntShares.Block.GetTransaction", Block_GetTransaction);
            Register("AntShares.Transaction.GetHash", Transaction_GetHash);
            Register("AntShares.Transaction.GetType", Transaction_GetType);
            Register("AntShares.Transaction.GetAttributes", Transaction_GetAttributes);
            Register("AntShares.Transaction.GetInputs", Transaction_GetInputs);
            Register("AntShares.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("AntShares.Transaction.GetReferences", Transaction_GetReferences);
            Register("AntShares.Attribute.GetUsage", Attribute_GetUsage);
            Register("AntShares.Attribute.GetData", Attribute_GetData);
            Register("AntShares.Input.GetHash", Input_GetHash);
            Register("AntShares.Input.GetIndex", Input_GetIndex);
            Register("AntShares.Output.GetAssetId", Output_GetAssetId);
            Register("AntShares.Output.GetValue", Output_GetValue);
            Register("AntShares.Output.GetScriptHash", Output_GetScriptHash);
            Register("AntShares.Account.GetScriptHash", Account_GetScriptHash);
            Register("AntShares.Account.GetVotes", Account_GetVotes);
            Register("AntShares.Account.GetBalance", Account_GetBalance);
            Register("AntShares.Asset.Create", Asset_Create);
            Register("AntShares.Asset.Renew", Asset_Renew);
            Register("AntShares.Asset.GetAssetId", Asset_GetAssetId);
            Register("AntShares.Asset.GetAssetType", Asset_GetAssetType);
            Register("AntShares.Asset.GetAmount", Asset_GetAmount);
            Register("AntShares.Asset.GetAvailable", Asset_GetAvailable);
            Register("AntShares.Asset.GetPrecision", Asset_GetPrecision);
            Register("AntShares.Asset.GetOwner", Asset_GetOwner);
            Register("AntShares.Asset.GetAdmin", Asset_GetAdmin);
            Register("AntShares.Asset.GetIssuer", Asset_GetIssuer);
            Register("AntShares.Contract.Create", Contract_Create);
            Register("AntShares.Contract.Migrate", Contract_Migrate);
            Register("AntShares.Contract.Destroy", Contract_Destroy);
            Register("AntShares.Contract.GetScript", Contract_GetScript);
            Register("AntShares.Contract.GetStorageContext", Contract_GetStorageContext);
            Register("AntShares.Storage.GetContext", Storage_GetContext);
            Register("AntShares.Storage.Get", Storage_Get);
            Register("AntShares.Storage.Put", Storage_Put);
            Register("AntShares.Storage.Delete", Storage_Delete);
        }

        private bool Blockchain_GetAccount(ExecutionEngine engine)
        {
            UInt160 hash = new UInt160(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            AccountState account = Snapshot.Accounts.GetOrAdd(hash, () => new AccountState(hash));
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(account));
            return true;
        }

        private bool Blockchain_GetValidators(ExecutionEngine engine)
        {
            ECPoint[] validators = Snapshot.GetValidators();
            engine.CurrentContext.EvaluationStack.Push(validators.Select(p => (StackItem)p.EncodePoint(true)).ToArray());
            return true;
        }

        private bool Blockchain_GetAsset(ExecutionEngine engine)
        {
            UInt256 hash = new UInt256(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            AssetState asset = Snapshot.Assets.TryGet(hash);
            if (asset == null) return false;
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(asset));
            return true;
        }

        private bool Header_GetVersion(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                BlockBase header = _interface.GetInterface<BlockBase>();
                if (header == null) return false;
                engine.CurrentContext.EvaluationStack.Push(header.Version);
                return true;
            }
            return false;
        }

        private bool Header_GetMerkleRoot(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                BlockBase header = _interface.GetInterface<BlockBase>();
                if (header == null) return false;
                engine.CurrentContext.EvaluationStack.Push(header.MerkleRoot.ToArray());
                return true;
            }
            return false;
        }

        private bool Header_GetConsensusData(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                BlockBase header = _interface.GetInterface<BlockBase>();
                if (header == null) return false;
                engine.CurrentContext.EvaluationStack.Push(header.ConsensusData);
                return true;
            }
            return false;
        }

        private bool Header_GetNextConsensus(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                BlockBase header = _interface.GetInterface<BlockBase>();
                if (header == null) return false;
                engine.CurrentContext.EvaluationStack.Push(header.NextConsensus.ToArray());
                return true;
            }
            return false;
        }

        private bool Transaction_GetType(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                engine.CurrentContext.EvaluationStack.Push((int)tx.Type);
                return true;
            }
            return false;
        }

        private bool Transaction_GetAttributes(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                if (tx.Attributes.Length > ApplicationEngine.MaxArraySize)
                    return false;
                engine.CurrentContext.EvaluationStack.Push(tx.Attributes.Select(p => StackItem.FromInterface(p)).ToArray());
                return true;
            }
            return false;
        }

        private bool Transaction_GetInputs(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                if (tx.Inputs.Length > ApplicationEngine.MaxArraySize)
                    return false;
                engine.CurrentContext.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(p)).ToArray());
                return true;
            }
            return false;
        }

        private bool Transaction_GetOutputs(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                if (tx.Outputs.Length > ApplicationEngine.MaxArraySize)
                    return false;
                engine.CurrentContext.EvaluationStack.Push(tx.Outputs.Select(p => StackItem.FromInterface(p)).ToArray());
                return true;
            }
            return false;
        }

        private bool Transaction_GetReferences(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                if (tx.Inputs.Length > ApplicationEngine.MaxArraySize)
                    return false;
                engine.CurrentContext.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(tx.References[p])).ToArray());
                return true;
            }
            return false;
        }

        private bool Transaction_GetUnspentCoins(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                Transaction tx = _interface.GetInterface<Transaction>();
                if (tx == null) return false;
                TransactionOutput[] outputs = Snapshot.GetUnspent(tx.Hash).ToArray();
                if (outputs.Length > ApplicationEngine.MaxArraySize)
                    return false;
                engine.CurrentContext.EvaluationStack.Push(outputs.Select(p => StackItem.FromInterface(p)).ToArray());
                return true;
            }
            return false;
        }

        private bool InvocationTransaction_GetScript(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                InvocationTransaction tx = _interface.GetInterface<InvocationTransaction>();
                if (tx == null) return false;
                engine.CurrentContext.EvaluationStack.Push(tx.Script);
                return true;
            }
            return false;
        }

        private bool Attribute_GetUsage(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                TransactionAttribute attr = _interface.GetInterface<TransactionAttribute>();
                if (attr == null) return false;
                engine.CurrentContext.EvaluationStack.Push((int)attr.Usage);
                return true;
            }
            return false;
        }

        private bool Attribute_GetData(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                TransactionAttribute attr = _interface.GetInterface<TransactionAttribute>();
                if (attr == null) return false;
                engine.CurrentContext.EvaluationStack.Push(attr.Data);
                return true;
            }
            return false;
        }

        private bool Input_GetHash(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                CoinReference input = _interface.GetInterface<CoinReference>();
                if (input == null) return false;
                engine.CurrentContext.EvaluationStack.Push(input.PrevHash.ToArray());
                return true;
            }
            return false;
        }

        private bool Input_GetIndex(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                CoinReference input = _interface.GetInterface<CoinReference>();
                if (input == null) return false;
                engine.CurrentContext.EvaluationStack.Push((int)input.PrevIndex);
                return true;
            }
            return false;
        }

        private bool Output_GetAssetId(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                TransactionOutput output = _interface.GetInterface<TransactionOutput>();
                if (output == null) return false;
                engine.CurrentContext.EvaluationStack.Push(output.AssetId.ToArray());
                return true;
            }
            return false;
        }

        private bool Output_GetValue(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                TransactionOutput output = _interface.GetInterface<TransactionOutput>();
                if (output == null) return false;
                engine.CurrentContext.EvaluationStack.Push(output.Value.GetData());
                return true;
            }
            return false;
        }

        private bool Output_GetScriptHash(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                TransactionOutput output = _interface.GetInterface<TransactionOutput>();
                if (output == null) return false;
                engine.CurrentContext.EvaluationStack.Push(output.ScriptHash.ToArray());
                return true;
            }
            return false;
        }

        private bool Account_GetScriptHash(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AccountState account = _interface.GetInterface<AccountState>();
                if (account == null) return false;
                engine.CurrentContext.EvaluationStack.Push(account.ScriptHash.ToArray());
                return true;
            }
            return false;
        }

        private bool Account_GetVotes(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AccountState account = _interface.GetInterface<AccountState>();
                if (account == null) return false;
                engine.CurrentContext.EvaluationStack.Push(account.Votes.Select(p => (StackItem)p.EncodePoint(true)).ToArray());
                return true;
            }
            return false;
        }

        private bool Account_GetBalance(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AccountState account = _interface.GetInterface<AccountState>();
                UInt256 asset_id = new UInt256(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
                if (account == null) return false;
                Fixed8 balance = account.Balances.TryGetValue(asset_id, out Fixed8 value) ? value : Fixed8.Zero;
                engine.CurrentContext.EvaluationStack.Push(balance.GetData());
                return true;
            }
            return false;
        }

        private bool Asset_Create(ExecutionEngine engine)
        {
            if (Trigger != TriggerType.Application) return false;
            InvocationTransaction tx = (InvocationTransaction)engine.ScriptContainer;
            AssetType asset_type = (AssetType)(byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            if (!Enum.IsDefined(typeof(AssetType), asset_type) || asset_type == AssetType.CreditFlag || asset_type == AssetType.DutyFlag || asset_type == AssetType.GoverningToken || asset_type == AssetType.UtilityToken)
                return false;
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 1024)
                return false;
            string name = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            Fixed8 amount = new Fixed8((long)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger());
            if (amount == Fixed8.Zero || amount < -Fixed8.Penny) return false;
            if (asset_type == AssetType.Invoice && amount != -Fixed8.Penny)
                return false;
            byte precision = (byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            if (precision > 8) return false;
            if (asset_type == AssetType.Share && precision != 0) return false;
            if (amount != -Fixed8.Penny && amount.GetData() % (long)Math.Pow(10, 8 - precision) != 0)
                return false;
            ECPoint owner = ECPoint.DecodePoint(engine.CurrentContext.EvaluationStack.Pop().GetByteArray(), ECCurve.Secp256r1);
            if (owner.IsInfinity) return false;
            if (!CheckWitness(engine, owner))
                return false;
            UInt160 admin = new UInt160(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            UInt160 issuer = new UInt160(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            AssetState asset = Snapshot.Assets.GetOrAdd(tx.Hash, () => new AssetState
            {
                AssetId = tx.Hash,
                AssetType = asset_type,
                Name = name,
                Amount = amount,
                Available = Fixed8.Zero,
                Precision = precision,
                Fee = Fixed8.Zero,
                FeeAddress = new UInt160(),
                Owner = owner,
                Admin = admin,
                Issuer = issuer,
                Expiration = Snapshot.Height + 1 + 2000000,
                IsFrozen = false
            });
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(asset));
            return true;
        }

        private bool Asset_Renew(ExecutionEngine engine)
        {
            if (Trigger != TriggerType.Application) return false;
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                byte years = (byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
                asset = Snapshot.Assets.GetAndChange(asset.AssetId);
                if (asset.Expiration < Snapshot.Height + 1)
                    asset.Expiration = Snapshot.Height + 1;
                try
                {
                    asset.Expiration = checked(asset.Expiration + years * 2000000u);
                }
                catch (OverflowException)
                {
                    asset.Expiration = uint.MaxValue;
                }
                engine.CurrentContext.EvaluationStack.Push(asset.Expiration);
                return true;
            }
            return false;
        }

        private bool Asset_GetAssetId(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.AssetId.ToArray());
                return true;
            }
            return false;
        }

        private bool Asset_GetAssetType(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push((int)asset.AssetType);
                return true;
            }
            return false;
        }

        private bool Asset_GetAmount(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.Amount.GetData());
                return true;
            }
            return false;
        }

        private bool Asset_GetAvailable(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.Available.GetData());
                return true;
            }
            return false;
        }

        private bool Asset_GetPrecision(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push((int)asset.Precision);
                return true;
            }
            return false;
        }

        private bool Asset_GetOwner(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.Owner.EncodePoint(true));
                return true;
            }
            return false;
        }

        private bool Asset_GetAdmin(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.Admin.ToArray());
                return true;
            }
            return false;
        }

        private bool Asset_GetIssuer(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                AssetState asset = _interface.GetInterface<AssetState>();
                if (asset == null) return false;
                engine.CurrentContext.EvaluationStack.Push(asset.Issuer.ToArray());
                return true;
            }
            return false;
        }

        private bool Contract_Create(ExecutionEngine engine)
        {
            if (Trigger != TriggerType.Application) return false;
            byte[] script = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            if (script.Length > 1024 * 1024) return false;
            ContractParameterType[] parameter_list = engine.CurrentContext.EvaluationStack.Pop().GetByteArray().Select(p => (ContractParameterType)p).ToArray();
            if (parameter_list.Length > 252) return false;
            ContractParameterType return_type = (ContractParameterType)(byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            ContractPropertyState contract_properties = (ContractPropertyState)(byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string name = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string version = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string author = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string email = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 65536) return false;
            string description = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            UInt160 hash = script.ToScriptHash();
            ContractState contract = Snapshot.Contracts.TryGet(hash);
            if (contract == null)
            {
                contract = new ContractState
                {
                    Script = script,
                    ParameterList = parameter_list,
                    ReturnType = return_type,
                    ContractProperties = contract_properties,
                    Name = name,
                    CodeVersion = version,
                    Author = author,
                    Email = email,
                    Description = description
                };
                Snapshot.Contracts.Add(hash, contract);
                ContractsCreated.Add(hash, new UInt160(engine.CurrentContext.ScriptHash));
            }
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(contract));
            return true;
        }

        private bool Contract_Migrate(ExecutionEngine engine)
        {
            if (Trigger != TriggerType.Application) return false;
            byte[] script = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            if (script.Length > 1024 * 1024) return false;
            ContractParameterType[] parameter_list = engine.CurrentContext.EvaluationStack.Pop().GetByteArray().Select(p => (ContractParameterType)p).ToArray();
            if (parameter_list.Length > 252) return false;
            ContractParameterType return_type = (ContractParameterType)(byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            ContractPropertyState contract_properties = (ContractPropertyState)(byte)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string name = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string version = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string author = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 252) return false;
            string email = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            if (engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length > 65536) return false;
            string description = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.Pop().GetByteArray());
            UInt160 hash = script.ToScriptHash();
            ContractState contract = Snapshot.Contracts.TryGet(hash);
            if (contract == null)
            {
                contract = new ContractState
                {
                    Script = script,
                    ParameterList = parameter_list,
                    ReturnType = return_type,
                    ContractProperties = contract_properties,
                    Name = name,
                    CodeVersion = version,
                    Author = author,
                    Email = email,
                    Description = description
                };
                Snapshot.Contracts.Add(hash, contract);
                ContractsCreated.Add(hash, new UInt160(engine.CurrentContext.ScriptHash));
                if (contract.HasStorage)
                {
                    foreach (var pair in Snapshot.Storages.Find(engine.CurrentContext.ScriptHash).ToArray())
                    {
                        Snapshot.Storages.Add(new StorageKey
                        {
                            ScriptHash = hash,
                            Key = pair.Key.Key
                        }, new StorageItem
                        {
                            Value = pair.Value.Value
                        });
                    }
                }
            }
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(contract));
            return Contract_Destroy(engine);
        }

        private bool Contract_GetScript(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                ContractState contract = _interface.GetInterface<ContractState>();
                if (contract == null) return false;
                engine.CurrentContext.EvaluationStack.Push(contract.Script);
                return true;
            }
            return false;
        }

        private bool Contract_IsPayable(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                ContractState contract = _interface.GetInterface<ContractState>();
                if (contract == null) return false;
                engine.CurrentContext.EvaluationStack.Push(contract.Payable);
                return true;
            }
            return false;
        }

        private bool Storage_Find(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                StorageContext context = _interface.GetInterface<StorageContext>();
                if (!CheckStorageContext(context)) return false;
                byte[] prefix = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
                byte[] prefix_key;
                using (MemoryStream ms = new MemoryStream())
                {
                    int index = 0;
                    int remain = prefix.Length;
                    while (remain >= 16)
                    {
                        ms.Write(prefix, index, 16);
                        ms.WriteByte(0);
                        index += 16;
                        remain -= 16;
                    }
                    if (remain > 0)
                        ms.Write(prefix, index, remain);
                    prefix_key = context.ScriptHash.ToArray().Concat(ms.ToArray()).ToArray();
                }
                StorageIterator iterator = new StorageIterator(Snapshot.Storages.Find(prefix_key).Where(p => p.Key.Key.Take(prefix.Length).SequenceEqual(prefix)).GetEnumerator());
                engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(iterator));
                Disposables.Add(iterator);
                return true;
            }
            return false;
        }

        private bool Enumerator_Create(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is VMArray array)
            {
                IEnumerator enumerator = new ArrayWrapper(array);
                engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(enumerator));
                return true;
            }
            return false;
        }

        private bool Enumerator_Next(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                IEnumerator enumerator = _interface.GetInterface<IEnumerator>();
                engine.CurrentContext.EvaluationStack.Push(enumerator.Next());
                return true;
            }
            return false;
        }

        private bool Enumerator_Value(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                IEnumerator enumerator = _interface.GetInterface<IEnumerator>();
                engine.CurrentContext.EvaluationStack.Push(enumerator.Value());
                return true;
            }
            return false;
        }

        private bool Enumerator_Concat(ExecutionEngine engine)
        {
            if (!(engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface1)) return false;
            if (!(engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface2)) return false;
            IEnumerator first = _interface1.GetInterface<IEnumerator>();
            IEnumerator second = _interface2.GetInterface<IEnumerator>();
            IEnumerator result = new ConcatenatedEnumerator(first, second);
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(result));
            return true;
        }

        private bool Iterator_Create(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is Map map)
            {
                IIterator iterator = new MapWrapper(map);
                engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(iterator));
                return true;
            }
            return false;
        }

        private bool Iterator_Key(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                IIterator iterator = _interface.GetInterface<IIterator>();
                engine.CurrentContext.EvaluationStack.Push(iterator.Key());
                return true;
            }
            return false;
        }

        private bool Iterator_Keys(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                IIterator iterator = _interface.GetInterface<IIterator>();
                engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new IteratorKeysWrapper(iterator)));
                return true;
            }
            return false;
        }

        private bool Iterator_Values(ExecutionEngine engine)
        {
            if (engine.CurrentContext.EvaluationStack.Pop() is InteropContract _interface)
            {
                IIterator iterator = _interface.GetInterface<IIterator>();
                engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new IteratorValuesWrapper(iterator)));
                return true;
            }
            return false;
        }
    }
}
