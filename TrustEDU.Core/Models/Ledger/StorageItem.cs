using System.IO;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Models.Ledger
{
    public class StorageItem : StateBase, ICloneable<StorageItem>
    {
        public byte[] Value;

        public override int Size => base.Size + Value.GetVarSize();

        StorageItem ICloneable<StorageItem>.Clone()
        {
            return new StorageItem
            {
                Value = Value
            };
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Value = reader.ReadVarBytes();
        }

        void ICloneable<StorageItem>.FromReplica(StorageItem replica)
        {
            Value = replica.Value;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteVarBytes(Value);
        }
    }
}