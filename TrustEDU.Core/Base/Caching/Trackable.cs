namespace TrustEDU.Core.Base.Caching
{
    internal class Trackable<TKey, TValue>
    {
        public TKey Key;
        public TValue Item;
        public TrackState State;
    }
}
