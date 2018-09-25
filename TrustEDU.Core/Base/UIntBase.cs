using System;
using System.IO;
using System.Linq;
using TrustEDU.Core.Base.IO;

namespace TrustEDU.Core.Base
{
    public class UIntBase : IEquatable<UIntBase>, ISerializable
    {
        private byte[] dataInBytes;

        public int Size => dataInBytes.Length;

        protected UIntBase(int bytes, byte[] value)
        {
            if (value == null)
            {
                this.dataInBytes = new byte[bytes];
                return;
            }
            if (value.Length != bytes)
            {
                throw new ArgumentException();
            }

            this.dataInBytes = value;
        }

        void ISerializable.Deserialize(BinaryReader reader)
        {
            reader.Read(dataInBytes, 0, dataInBytes.Length);
        }

        public bool Equals(UIntBase other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (dataInBytes.Length != other.dataInBytes.Length)
                return false;
            return dataInBytes.SequenceEqual(other.dataInBytes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is UIntBase))
                return false;
            return this.Equals((UIntBase)obj);
        }

        public override int GetHashCode()
        {
            return dataInBytes.ToInt32(0);
        }

        public static UIntBase Parse(string s)
        {
            if (s.Length == 40 || s.Length == 42)
                return UInt160.Parse(s);
            else if (s.Length == 64 || s.Length == 66)
                return UInt256.Parse(s);
            else
                throw new FormatException();
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(dataInBytes);
        }

        public byte[] ToArray()
        {
            return dataInBytes;
        }

        public override string ToString()
        {
            return "0x" + dataInBytes.Reverse().ToHexString();
        }

        public static bool TryParse<T>(string s, out T result) where T : UIntBase
        {
            int size;
            if (typeof(T) == typeof(UInt160))
                size = 20;
            else if (typeof(T) == typeof(UInt256))
                size = 32;
            else if (s.Length == 40 || s.Length == 42)
                size = 20;
            else if (s.Length == 64 || s.Length == 66)
                size = 32;
            else
                size = 0;
            if (size == 20)
            {
                if (UInt160.TryParse(s, out UInt160 r))
                {
                    result = (T)(UIntBase)r;
                    return true;
                }
            }
            else if (size == 32)
            {
                if (UInt256.TryParse(s, out UInt256 r))
                {
                    result = (T)(UIntBase)r;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public static bool operator ==(UIntBase left, UIntBase right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(UIntBase left, UIntBase right)
        {
            return !(left == right);
        }
    }
}
