﻿using TrustEDU.Core.Base;
using TrustEDU.Core.Models.SmartContract;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.Ledger
{
    public class ApplicationExecutionResult
    {
        public TriggerType Trigger { get; internal set; }
        public UInt160 ScriptHash { get; internal set; }
        public VMState VMState { get; internal set; }
        public Fixed8 GasConsumed { get; internal set; }
        public StackItem[] Stack { get; internal set; }
        public NotifyEventArgs[] Notifications { get; internal set; }
    }
}