using System.Collections.Generic;
using TrustEDU.Core.Models.Transactions;

namespace TrustEDU.Core.Plugins
{
    public interface IPolicyPlugin
    {
        bool FilterForMemoryPool(Transaction tx);
        IEnumerable<Transaction> FilterForBlock(IEnumerable<Transaction> transactions);
    }
}