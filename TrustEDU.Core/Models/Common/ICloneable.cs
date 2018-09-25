namespace TrustEDU.Core.Models.Common
{
    public interface ICloneable<T>
    {
        T Clone();
        void FromReplica(T replica);
    }
}