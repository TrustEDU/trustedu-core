using System;
using System.IO;

namespace TrustEDU.Core.Base.Types
{
    public sealed class UInt32Wrapper : SerializableWrapper<uint>, IEquatable<UInt32Wrapper>
    {
        public override int Size => sizeof(uint);

        public UInt32Wrapper()
        {
        }

        private UInt32Wrapper(uint value)
        {
            this.Value = value;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Value = reader.ReadUInt32();
        }

        public bool Equals(UInt32Wrapper other)
        {
            return other != null && Value == other.Value;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public static implicit operator UInt32Wrapper(uint value)
        {
            return new UInt32Wrapper(value);
        }

        public static implicit operator uint(UInt32Wrapper wrapper)
        {
            return wrapper.Value;
        }
    }
}
