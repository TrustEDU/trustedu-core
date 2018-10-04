namespace TrustEDU.Core.Models.Inventory
{
	public enum InventoryType: byte
    {
        Tx = 0x01,
        Block = 0x02,
        Consensus = 0xe0
    }
}