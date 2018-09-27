using System;
using System.Net;
using Akka.Actor;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.Wallets;
using TrustEDU.Core.Network.Peer2Peer;
using TrustEDU.Core.Network.RPC;
using TrustEDU.Core.Persistence;
using TrustEDU.Core.Plugins;

namespace TrustEDU.Core
{
    public class TrustEDUNetwork : IDisposable
    {
        public readonly ActorSystem ActorSystem = ActorSystem.Create(nameof(TrustEDUNetwork),
            $"akka {{ log-dead-letters = off }}" +
            $"blockchain-mailbox {{ mailbox-type: \"{typeof(BlockchainMailbox).AssemblyQualifiedName}\" }}" +
            $"task-manager-mailbox {{ mailbox-type: \"{typeof(TaskManagerMailbox).AssemblyQualifiedName}\" }}" +
            $"remote-node-mailbox {{ mailbox-type: \"{typeof(RemoteNodeMailbox).AssemblyQualifiedName}\" }}" +
            $"protocol-handler-mailbox {{ mailbox-type: \"{typeof(ProtocolHandlerMailbox).AssemblyQualifiedName}\" }}" +
            $"consensus-service-mailbox {{ mailbox-type: \"{typeof(ConsensusServiceMailbox).AssemblyQualifiedName}\" }}");
        public readonly IActorRef Blockchain;
        public readonly IActorRef LocalNode;
        internal readonly IActorRef TaskManager;
        internal IActorRef Consensus;
        private RpcServer rpcServer;

        public TrustEDUNetwork(Store store)
        {
            this.Blockchain = ActorSystem.ActorOf(Logger.Blockchain.Props(this, store));
            this.LocalNode = ActorSystem.ActorOf(LocalNode.Props(this));
            this.TaskManager = ActorSystem.ActorOf(TaskManager.Props(this));
            Plugin.LoadPlugins(this);
        }

        public void Dispose()
        {
            rpcServer?.Dispose();
            ActorSystem.Stop(LocalNode);
            ActorSystem.Dispose();
        }

        public void StartConsensus(Wallet wallet)
        {
            Consensus = ActorSystem.ActorOf(ConsensusService.Props(this, wallet));
            Consensus.Tell(new ConsensusService.Start());
        }

        public void StartNode(int port = 0, int ws_port = 0)
        {
            LocalNode.Tell(new Peer.Start
            {
                Port = port,
                WsPort = ws_port
            });
        }

        public void StartRpc(IPAddress bindAddress, int port, Wallet wallet = null, string sslCert = null, string password = null, string[] trustedAuthorities = null)
        {
            rpcServer = new RpcServer(this, wallet);
            rpcServer.Start(bindAddress, port, sslCert, password, trustedAuthorities);
        }
    }
}
