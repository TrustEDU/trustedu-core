using System.IO;

namespace TrustEDU.Core.Models.Common
{
    public interface ISerializable
    {
        int Size { get; }
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}