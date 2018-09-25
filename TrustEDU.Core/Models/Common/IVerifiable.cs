using System.IO;
using TrustEDU.Core.Base;
using TrustEDU.Core.IO.Persistence.LevelDB;
using TrustEDU.Core.Models.Inventory;
using TrustEDU.VM.Runtime;

namespace TrustEDU.Core.Models.Common
{
    public interface IVerifiable : ISerializable, IScriptContainer
    {
        Witness[] Witnesses { get; set; }

        void DeserializeUnsigned(BinaryReader reader);

        UInt160[] GetScriptHashesForVerifying(Snapshot snapshot);

        void SerializeUnsigned(BinaryWriter writer);
    }
}