using System.IO;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Inventory;
using TrustEDU.Core.Persistence;
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