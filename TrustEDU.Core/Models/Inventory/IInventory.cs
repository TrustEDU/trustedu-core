using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Common;
using TrustEDU.Core.Persistence;

namespace TrustEDU.Core.Models.Inventory
{
    public interface IInventory : IVerifiable
    {
        UInt256 Hash { get; }

        InventoryType InventoryType { get; }

        bool Verify(Snapshot snapshot);
    }
}
