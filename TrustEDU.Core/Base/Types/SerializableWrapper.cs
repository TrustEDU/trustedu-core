using System;
using System.IO;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Base.Types
{
    public abstract class SerializableWrapper<T> : IEquatable<T>, IEquatable<SerializableWrapper<T>>, ISerializable
        where T : struct, IEquatable<T>
    {
        protected T Value;

        public abstract int Size { get; }

        public abstract void Deserialize(BinaryReader reader);

        public bool Equals(T other)
        {
            return Value.Equals(other);
        }

        public bool Equals(SerializableWrapper<T> other)
        {
            return other != null && Value.Equals(other.Value);
        }

        public abstract void Serialize(BinaryWriter writer);
    }
}
