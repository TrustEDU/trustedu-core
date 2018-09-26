using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Inventory;

namespace TrustEDU.Core.Base.Caching
{
    internal class RelayCache : FIFOCache<UInt256, IInventory>
    {
        public RelayCache(int maxCapacity)
            : base(maxCapacity)
        {
        }

        protected override UInt256 GetKeyForItem(IInventory item)
        {
            return item.Hash;
        }
    }
}
