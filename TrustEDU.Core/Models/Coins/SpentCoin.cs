using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Transactions;

namespace TrustEDU.Core.Models.Coins
{
    public class SpentCoin
    {
        public TransactionOutput Output;
        public uint StartHeight;
        public uint EndHeight;

        public Fixed8 Value => Output.Value;
    }
}
