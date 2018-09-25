namespace TrustEDU.Core.Base.Caching
{
    internal abstract class FIFOCache<TKey, TValue> : Cache<TKey, TValue>
    {
        public FIFOCache(int maxCapacity)
            : base(maxCapacity)
        {
        }

        protected override void OnAccess(CacheItem item)
        {
        }
    }
}
