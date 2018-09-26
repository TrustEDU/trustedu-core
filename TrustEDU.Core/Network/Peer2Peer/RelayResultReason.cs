namespace TrustEDU.Core.Network.Peer2Peer
{
    public enum RelayResultReason : byte
    {
        Succeed,
        AlreadyExists,
        OutOfMemory,
        UnableToVerify,
        Invalid,
        Unknown
    }
}