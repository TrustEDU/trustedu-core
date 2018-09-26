using System;
using System.Runtime.InteropServices;
using TrustEDU.Core.Models.Exceptions;

namespace TrustEDU.Core.Models.LevelDB
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
