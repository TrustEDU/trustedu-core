using TrustEDU.Core.Base.Types;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.SmartContract
{
    internal class StorageContext : IInteropContract
    {
        public UInt160 ScriptHash;
        public bool IsReadOnly;

        public byte[] ToArray()
        {
            return ScriptHash.ToArray();
        }
    }
}
