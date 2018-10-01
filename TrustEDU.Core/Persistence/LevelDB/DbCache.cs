using System;
using System.Collections.Generic;
using TrustEDU.Core.Base.Caching;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Models.Common;
using TrustEDU.Core.Models.LevelDB;

namespace TrustEDU.Core.Persistence.LevelDB
{
    internal class DbCache<TKey, TValue> : CacheItem<TKey, TValue>
        where TKey : IEquatable<TKey>, ISerializable, new()
        where TValue : class, ICloneable<TValue>, ISerializable, new()
    {
        private readonly Database db;
        private readonly ReadOptions options;
        private readonly WriteBatch batch;
        private readonly byte prefix;

        public DbCache(Database db, ReadOptions options, WriteBatch batch, byte prefix)
        {
            this.db = db;
            this.options = options ?? ReadOptions.Default;
            this.batch = batch;
            this.prefix = prefix;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            batch?.Put(prefix, key, value);
        }

        public override void DeleteInternal(TKey key)
        {
            batch?.Delete(prefix, key);
        }

        protected override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] keyPrefix)
        {
            return db.Find(options, SliceBuilder.Begin(prefix).Add(keyPrefix), (k, v) => new KeyValuePair<TKey, TValue>(k.ToArray().AsSerializable<TKey>(1), v.ToArray().AsSerializable<TValue>()));
        }

        protected override TValue GetInternal(TKey key)
        {
            return db.Get<TValue>(options, prefix, key);
        }

        protected override TValue TryGetInternal(TKey key)
        {
            return db.TryGet<TValue>(options, prefix, key);
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            batch?.Put(prefix, key, value);
        }
    }
}