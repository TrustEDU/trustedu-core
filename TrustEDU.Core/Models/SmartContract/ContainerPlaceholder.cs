using System;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.SmartContract
{
    internal class ContainerPlaceholder : StackItem
    {
        public StackItemType Type;
        public int ElementCount;

        public override bool Equals(StackItem other)
        {
            throw new NotSupportedException();
        }

        public override byte[] GetByteArray()
        {
            throw new NotSupportedException();
        }
    }
}