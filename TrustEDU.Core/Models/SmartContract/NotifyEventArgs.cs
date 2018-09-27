using System;
using TrustEDU.Core.Base.Types;
using TrustEDU.VM.Base;
using TrustEDU.VM.Runtime;

namespace TrustEDU.Core.Models.SmartContract
{
    public class NotifyEventArgs : EventArgs
    {
        public IScriptContainer ScriptContainer { get; }
        public UInt160 ScriptHash { get; }
        public StackItem State { get; }

        public NotifyEventArgs(IScriptContainer container, UInt160 scriptHash, StackItem state)
        {
            this.ScriptContainer = container;
            this.ScriptHash = scriptHash;
            this.State = state;
        }
    }
}