using System;
using System.IO;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Base.Types
{
    public abstract class SerializableWrapper<T> : IEquatable<T>, IEquatable<SerializableWrapper<T>>, ISerializable
        where T : struct, IEquatable<T>
    {
        protected T value;

        public abstract int Size { get; }

        public abstract void Deserialize(BinaryReader reader);

        public bool Equals(T other)
        {
            return value.Equals(other);
        }

        public bool Equals(SerializableWrapper<T> other)
        {
            return value.Equals(other.value);
        }

        public abstract void Serialize(BinaryWriter writer);
    }
}
