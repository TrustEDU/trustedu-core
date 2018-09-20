namespace TrustEDU.Core.IO
{
    public interface ICloneable<T>
    {
        T Clone();
        void FromReplica(T replica);
    }
}