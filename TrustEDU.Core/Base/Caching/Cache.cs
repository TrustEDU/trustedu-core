using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TrustEDU.Core.Base.Caching
{
    internal abstract class Cache<TKey, TValue> : ICollection<TValue>, IDisposable
    {
        protected class CacheItem
        {
            public TKey Key;
            public TValue Value;
            public DateTime Time;

            public CacheItem(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
                this.Time = DateTime.Now;
            }
        }

        protected readonly ReaderWriterLockSlim RwSyncRootLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly Dictionary<TKey, CacheItem> InnerDictionary = new Dictionary<TKey, CacheItem>();
        private readonly int _maxCapacity;

        public TValue this[TKey key]
        {
            get
            {
                RwSyncRootLock.EnterReadLock();
                try
                {
                    if (!InnerDictionary.TryGetValue(key, out var item)) throw new KeyNotFoundException();
                    OnAccess(item);
                    return item.Value;
                }
                finally
                {
                    RwSyncRootLock.ExitReadLock();
                }
            }
        }

        public int Count
        {
            get
            {
                RwSyncRootLock.EnterReadLock();
                try
                {
                    return InnerDictionary.Count;
                }
                finally
                {
                    RwSyncRootLock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly => false;

        protected Cache(int maxCapacity)
        {
            this._maxCapacity = maxCapacity;
        }

        public void Add(TValue item)
        {
            var key = GetKeyForItem(item);
            RwSyncRootLock.EnterWriteLock();
            try
            {
                AddInternal(key, item);
            }
            finally
            {
                RwSyncRootLock.ExitWriteLock();
            }
        }

        private void AddInternal(TKey key, TValue item)
        {
            if (InnerDictionary.TryGetValue(key, out var cacheItem))
            {
                OnAccess(cacheItem);
            }
            else
            {
                if (InnerDictionary.Count >= _maxCapacity)
                {
                    foreach (var itemDel in InnerDictionary.Values.AsParallel().OrderBy(p => p.Time).Take(InnerDictionary.Count - _maxCapacity + 1))
                    {
                        RemoveInternal(itemDel);
                    }
                }
                InnerDictionary.Add(key, new CacheItem(key, item));
            }
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            RwSyncRootLock.EnterWriteLock();
            try
            {
                foreach (var item in items)
                {
                    var key = GetKeyForItem(item);
                    AddInternal(key, item);
                }
            }
            finally
            {
                RwSyncRootLock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            RwSyncRootLock.EnterWriteLock();
            try
            {
                foreach (var itemDel in InnerDictionary.Values.ToArray())
                {
                    RemoveInternal(itemDel);
                }
            }
            finally
            {
                RwSyncRootLock.ExitWriteLock();
            }
        }

        public bool Contains(TKey key)
        {
            RwSyncRootLock.EnterReadLock();
            try
            {
                if (!InnerDictionary.TryGetValue(key, out var cacheItem)) return false;
                OnAccess(cacheItem);
                return true;
            }
            finally
            {
                RwSyncRootLock.ExitReadLock();
            }
        }

        public bool Contains(TValue item)
        {
            return Contains(GetKeyForItem(item));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (arrayIndex + InnerDictionary.Count > array.Length) throw new ArgumentException();
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public void Dispose()
        {
            Clear();
            RwSyncRootLock.Dispose();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            RwSyncRootLock.EnterReadLock();
            try
            {
                foreach (TValue item in InnerDictionary.Values.Select(p => p.Value))
                {
                    yield return item;
                }
            }
            finally
            {
                RwSyncRootLock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract TKey GetKeyForItem(TValue item);

        public bool Remove(TKey key)
        {
            RwSyncRootLock.EnterWriteLock();
            try
            {
                if (!InnerDictionary.TryGetValue(key, out var cacheItem)) return false;
                RemoveInternal(cacheItem);
                return true;
            }
            finally
            {
                RwSyncRootLock.ExitWriteLock();
            }
        }

        protected abstract void OnAccess(CacheItem item);

        public bool Remove(TValue item)
        {
            return Remove(GetKeyForItem(item));
        }

        private void RemoveInternal(CacheItem item)
        {
            InnerDictionary.Remove(item.Key);
            if (item.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool TryGet(TKey key, out TValue item)
        {
            RwSyncRootLock.EnterReadLock();
            try
            {
                if (InnerDictionary.TryGetValue(key, out var cacheItem))
                {
                    OnAccess(cacheItem);
                    item = cacheItem.Value;
                    return true;
                }
            }
            finally
            {
                RwSyncRootLock.ExitReadLock();
            }
            item = default(TValue);
            return false;
        }
    }
}
