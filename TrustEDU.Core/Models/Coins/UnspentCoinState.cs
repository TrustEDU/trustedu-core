using System.IO;
using System.Linq;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Models.Coins
{
    public class UnspentCoinState : StateBase, ICloneable<UnspentCoinState>
    {
        public CoinState[] Items;

        public override int Size => base.Size + Items.GetVarSize();

        UnspentCoinState ICloneable<UnspentCoinState>.Clone()
        {
            return new UnspentCoinState
            {
                Items = (CoinState[])Items.Clone()
            };
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Items = reader.ReadVarBytes().Select(p => (CoinState)p).ToArray();
        }

        void ICloneable<UnspentCoinState>.FromReplica(UnspentCoinState replica)
        {
            Items = replica.Items;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteVarBytes(Items.Cast<byte>().ToArray());
        }
    }
}
