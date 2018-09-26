using System;

namespace TrustEDU.Core.Models.LevelDB
{
    public enum CompressionType: byte
    {
        kNoCompression = 0x0,
        kSnappyCompression = 0x1
    }
}
