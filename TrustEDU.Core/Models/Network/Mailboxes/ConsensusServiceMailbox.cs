using System;
using TrustEDU.Core.Models.Actors;
using TrustEDU.Core.Models.Ledger;

namespace TrustEDU.Core.Models.Network.Mailboxes
{
    internal class ConsensusServiceMailbox : PriorityMailbox
    {
        public ConsensusServiceMailbox(Akka.Actor.Settings settings, Config config)
            : base(settings, config)
        {
        }

        protected override bool IsHighPriority(object message)
        {
            switch (message)
            {
                case ConsensusPayload _:
                case ConsensusService.Timer _:
                case Blockchain.PersistCompleted _:
                    return true;
                default:
                    return false;
            }
        }
    }
}
