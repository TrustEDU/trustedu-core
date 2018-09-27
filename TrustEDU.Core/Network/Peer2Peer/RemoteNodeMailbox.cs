using Akka.Configuration;
using Akka.IO;
using TrustEDU.Core.Models.Actors;

namespace TrustEDU.Core.Network.Peer2Peer
{
    internal class RemoteNodeMailbox : PriorityMailbox
    {
        public RemoteNodeMailbox(Akka.Actor.Settings settings, Config config)
            : base(settings, config)
        {
        }

        protected override bool IsHighPriority(object message)
        {
            switch (message)
            {
                case Tcp.ConnectionClosed _:
                case Connection.Timer _:
                case Connection.Ack _:
                    return true;
                default:
                    return false;
            }
        }
    }
}
