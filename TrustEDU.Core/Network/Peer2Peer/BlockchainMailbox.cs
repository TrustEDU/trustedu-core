using Akka.Actor;
using Akka.Configuration;
using TrustEDU.Core.Models.Actors;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.Network;

namespace TrustEDU.Core.Network.Peer2Peer
{
    internal class BlockchainMailbox : PriorityMailbox
    {
        public BlockchainMailbox(Akka.Actor.Settings settings, Config config)
            : base(settings, config)
        {
        }

        protected override bool IsHighPriority(object message)
        {
            switch (message)
            {
                case Header[] _:
                case Block _:
                case ConsensusPayload _:
                case Terminated _:
                    return true;
                default:
                    return false;
            }
        }
    }
}
