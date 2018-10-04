using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;

namespace TrustEDU.Core.Models.Actors
{
    internal class PriorityMessageQueue : IMessageQueue, IUnboundedMessageQueueSemantics
    {
        private readonly ConcurrentQueue<Envelope> _high = new ConcurrentQueue<Envelope>();
        private readonly ConcurrentQueue<Envelope> _low = new ConcurrentQueue<Envelope>();
        private readonly Func<object, IEnumerable, bool> _dropper;
        private readonly Func<object, bool> _priorityGenerator;

        public bool HasMessages => !_high.IsEmpty || !_low.IsEmpty;
        public int Count => _high.Count + _low.Count;

        public PriorityMessageQueue(Func<object, IEnumerable, bool> dropper, Func<object, bool> priorityGenerator)
        {
            this._dropper = dropper;
            this._priorityGenerator = priorityGenerator;
        }

        public void CleanUp(IActorRef owner, IMessageQueue deadletters)
        {
        }

        public void Enqueue(IActorRef receiver, Envelope envelope)
        {
            if (_dropper(envelope.Message, _high.Concat(_low).Select(p => p.Message)))
                return;
            ConcurrentQueue<Envelope> queue = _priorityGenerator(envelope.Message) ? _high : _low;
            queue.Enqueue(envelope);
        }

        public bool TryDequeue(out Envelope envelope)
        {
            if (_high.TryDequeue(out envelope)) return true;
            return _low.TryDequeue(out envelope);
        }
    }
}