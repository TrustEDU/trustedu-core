using System;
using System.IO;
using TrustEDU.Core.Base.Json;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.Common
{
    public abstract class StateBase : IInteropContract, ISerializable
    {
        public const byte StateVersion = 0;

        public virtual int Size => sizeof(byte);

        public virtual void Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != StateVersion) throw new FormatException();
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(StateVersion);
        }

        public virtual JObject ToJson()
        {
            JObject json = new JObject();
            json["version"] = StateVersion;
            return json;
        }
    }
}
