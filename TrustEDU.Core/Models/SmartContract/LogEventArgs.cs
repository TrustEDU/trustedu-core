using System;
using TrustEDU.Core.Base.Types;
using TrustEDU.VM.Runtime;

namespace TrustEDU.Core.Models.SmartContract
{
    public class LogEventArgs : EventArgs
    {
        public IScriptContainer ScriptContainer { get; }
        public UInt160 ScriptHash { get; }
        public string Message { get; }

        public LogEventArgs(IScriptContainer container, UInt160 scriptHash, string message)
        {
            this.ScriptContainer = container;
            this.ScriptHash = scriptHash;
            this.Message = message;
        }
    }
}