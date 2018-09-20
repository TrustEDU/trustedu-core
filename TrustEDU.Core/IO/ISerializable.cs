using System.IO;

namespace TrustEDU.Core.IO
{
    public interface ISerializable
    {
        int Size { get; }
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}