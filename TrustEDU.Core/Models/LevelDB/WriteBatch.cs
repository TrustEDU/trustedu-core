using System;

namespace TrustEDU.Core.Models.LevelDB
{
    public class WriteBatch
    {
        internal readonly IntPtr handle = Native.leveldb_writebatch_create();

        public void Clear()
        {
            Native.leveldb_writebatch_clear(handle);
        }

        public void Delete(Slice key)
        {
            Native.leveldb_writebatch_delete(handle, key.buffer, (UIntPtr)key.buffer.Length);
        }

        public void Put(Slice key, Slice value)
        {
            Native.leveldb_writebatch_put(handle, key.buffer, (UIntPtr)key.buffer.Length, value.buffer, (UIntPtr)value.buffer.Length);
        }

        ~WriteBatch()
        {
            Native.leveldb_writebatch_destroy(handle);
        }
    }
}