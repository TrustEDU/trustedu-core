using System;
using System.Collections.Generic;
using Akka.Actor;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Network;

namespace TrustEDU.Core.Network.Peer2Peer
{
    internal class TaskSession
    {
        public readonly IActorRef RemoteNode;
        public readonly VersionPayload Version;
        public readonly Dictionary<UInt256, DateTime> Tasks = new Dictionary<UInt256, DateTime>();
        public readonly HashSet<UInt256> AvailableTasks = new HashSet<UInt256>();

        public bool HasTask => Tasks.Count > 0;
        public bool HeaderTask => Tasks.ContainsKey(UInt256.Zero);

        public TaskSession(IActorRef node, VersionPayload version)
        {
            this.RemoteNode = node;
            this.Version = version;
        }
    }
}
