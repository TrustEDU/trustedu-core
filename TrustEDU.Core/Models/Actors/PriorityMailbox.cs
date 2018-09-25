using System.Collections;
using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;

namespace TrustEDU.Core.Models.Actors
{
    internal abstract class PriorityMailbox : MailboxType, IProducesMessageQueue<PriorityMessageQueue>
    {
        public PriorityMailbox(Settings settings, Config config)
            : base(settings, config)
        {
        }

        public override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            return new PriorityMessageQueue(ShallDrop, IsHighPriority);
        }

        protected virtual bool IsHighPriority(object message) => false;
        protected virtual bool ShallDrop(object message, IEnumerable queue) => false;
    }
}
