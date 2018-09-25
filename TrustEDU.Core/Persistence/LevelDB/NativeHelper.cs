using System;
using TrustEDU.Core.IO.Persistence.LevelDB.Exceptions;

namespace TrustEDU.Core.IO.Persistence.LevelDB
{
    internal static class NativeHelper
    {
        public static void CheckError(IntPtr error)
        {
            if (error != IntPtr.Zero)
            {
                string message = Marshal.PtrToStringAnsi(error);
                Native.leveldb_free(error);
                throw new LevelDBException(message);
            }
        }
    }
}
