using System.IO;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Models.Network
{
    public class FilterAddPayload : ISerializable
    {
        public byte[] Data;

        public int Size => Data.GetVarSize();

        void ISerializable.Deserialize(BinaryReader reader)
        {
            Data = reader.ReadVarBytes(520);
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.WriteVarBytes(Data);
        }
    }
}
