using System;

namespace TrustEDU.Core.IO.Persistence.LevelDB
{
    public enum CompressionType: byte
    {
        kNoCompression = 0x0,
        kSnappyCompression = 0x1
    }
}
