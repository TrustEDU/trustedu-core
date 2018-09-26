using System;
using TrustEDU.Core.Models.Exceptions;

namespace TrustEDU.Core.Models.LevelDB
{
    public class Database: IDisposable
    {
        private IntPtr handle;

        /// <summary>
        /// Indicates whether or not the database is disposed with null pointer
        /// </summary>
        public bool IsDisposed => handle == IntPtr.Zero;

        private Database(IntPtr handle)
        {
            this.handle = handle;
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                Native.leveldb_close(handle);
                handle = IntPtr.Zero;
            }
        }

        public void Delete(WriteOptions options, Slice key)
        {
            Native.leveldb_delete(handle, options.handle, key.buffer, (UIntPtr)key.buffer.Length, out IntPtr error);
            NativeHelper.CheckError(error);
        }

        public Slice Get(ReadOptions options, Slice key)
        {
            IntPtr value = Native.leveldb_get(handle, options.handle, key.buffer, (UIntPtr)key.buffer.Length, out UIntPtr length, out IntPtr error);
            try
            {
                NativeHelper.CheckError(error);
                if (value == IntPtr.Zero)
                    throw new LevelDBException("Database not found");
                return new Slice(value, length);
            }
            finally
            {
                if (value != IntPtr.Zero) Native.leveldb_free(value);
            }
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(handle);
        }

        public Iterator NewIterator(ReadOptions options)
        {
            return new Iterator(Native.leveldb_create_iterator(handle, options.handle));
        }

        public static Database Open(string name)
        {
            return Open(name, Options.Default);
        }

        public static Database Open(string name, Options options)
        {
            IntPtr handle = Native.leveldb_open(options.handle, name, out IntPtr error);
            NativeHelper.CheckError(error);
            return new Database(handle);
        }

        public void Put(WriteOptions options, Slice key, Slice value)
        {
            Native.leveldb_put(handle, options.handle, key.buffer, (UIntPtr)key.buffer.Length, value.buffer, (UIntPtr)value.buffer.Length, out IntPtr error);
            NativeHelper.CheckError(error);
        }

        public bool TryGet(ReadOptions options, Slice key, out Slice value)
        {
            IntPtr v = Native.leveldb_get(handle, options.handle, key.buffer, (UIntPtr)key.buffer.Length, out UIntPtr length, out IntPtr error);
            if (error != IntPtr.Zero)
            {
                Native.leveldb_free(error);
                value = default(Slice);
                return false;
            }
            if (v == IntPtr.Zero)
            {
                value = default(Slice);
                return false;
            }
            value = new Slice(v, length);
            Native.leveldb_free(v);
            return true;
        }

        public void Write(WriteOptions options, WriteBatch write_batch)
        {
            byte retry = 0;
            while (true)
            {
                try
                {
                    Native.leveldb_write(handle, options.handle, write_batch.handle, out IntPtr error);
                    NativeHelper.CheckError(error);
                    break;
                }
                catch (LevelDBException ex)
                {
                    if (++retry >= 4) throw;
                    System.IO.File.AppendAllText("trustedu_db.log", ex.Message + "\r\n");
                }
            }
        }
    }
}