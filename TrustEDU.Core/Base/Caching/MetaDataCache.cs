using System;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Base.Caching
{
    public abstract class MetaDataCache<T>
        where T : class, ICloneable<T>, ISerializable, new()
    {
        private T _item;
        private TrackState _state;
        private readonly Func<T> _factory;

        protected abstract void AddInternal(T item);
        protected abstract T TryGetInternal();
        protected abstract void UpdateInternal(T item);

        protected MetaDataCache(Func<T> factory)
        {
            this._factory = factory;
        }

        public void Commit()
        {
            switch (_state)
            {
                case TrackState.Added:
                    AddInternal(_item);
                    break;
                case TrackState.Changed:
                    UpdateInternal(_item);
                    break;
                case TrackState.None:
                    break;
                case TrackState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public MetaDataCache<T> CreateSnapshot() => new CloneMetaCache<T>(this);

        public T Get()
        {
            if (_item == null)
            {
                _item = TryGetInternal();
            }
            if (_item != null) return _item;

            _item = _factory?.Invoke() ?? new T();
            _state = TrackState.Added;
            return _item;
        }

        public T GetAndChange()
        {
            var item = Get();
            if (_state == TrackState.None)
                _state = TrackState.Changed;
            return item;
        }
    }
}