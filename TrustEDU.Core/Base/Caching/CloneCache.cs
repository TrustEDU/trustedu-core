using System;
using System.Collections.Generic;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Base.Caching
{
    internal class CloneCache<TKey, TValue> : CacheItem<TKey, TValue>
        where TKey : IEquatable<TKey>, ISerializable
        where TValue : class, ICloneable<TValue>, ISerializable, new()
    {
        private readonly CacheItem<TKey, TValue> _innerCache;

        public CloneCache(CacheItem<TKey, TValue> innerCache)
        {
            this._innerCache = innerCache;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            _innerCache.Add(key, value);
        }

        public override void DeleteInternal(TKey key)
        {
            _innerCache.Delete(key);
        }

        protected override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] keyPrefix)
        {
            foreach (var pair in _innerCache.Find(keyPrefix))
            {
                yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Clone());
            }  
        }

        protected override TValue GetInternal(TKey key)
        {
            return _innerCache[key].Clone();
        }

        protected override TValue TryGetInternal(TKey key)
        {
            return _innerCache.TryGet(key)?.Clone();
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            _innerCache.GetAndChange(key).FromReplica(value);
        }
    }
}