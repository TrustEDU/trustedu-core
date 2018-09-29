using System.Collections;
using System.Linq;
using Akka.Configuration;
using TrustEDU.Core.Models.Actors;
using Message = TrustEDU.Core.Network.Peer2Peer.Message;

namespace TrustEDU.Core.Models.Network.Mailboxes
{
    internal class ProtocolHandlerMailbox : PriorityMailbox
    {
        public ProtocolHandlerMailbox(Akka.Actor.Settings settings, Config config)
            : base(settings, config)
        {
        }

        protected override bool IsHighPriority(object message)
        {
            if (!(message is Message msg)) return true;
            switch (msg.Command)
            {
                case "consensus":
                case "filteradd":
                case "filterclear":
                case "filterload":
                case "verack":
                case "version":
                case "alert":
                    return true;
                default:
                    return false;
            }
        }

        protected override bool ShallDrop(object message, IEnumerable queue)
        {
            if (!(message is Message msg)) return false;
            switch (msg.Command)
            {
                case "getaddr":
                case "getblocks":
                case "getdata":
                case "getheaders":
                case "mempool":
                    return queue.OfType<Message>().Any(p => p.Command == msg.Command);
                default:
                    return false;
            }
        }
    }
}
