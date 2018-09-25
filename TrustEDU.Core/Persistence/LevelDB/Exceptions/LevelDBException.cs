using System;
using System.Data.Common;

namespace TrustEDU.Core.IO.Persistence.LevelDB.Exceptions
{
    public class LevelDBException: DbException
    {
        internal LevelDBException(string message): base(message)
        {
        }

        public LevelDBException(string message, Exception innerException) 
            :base(message, innerException)
        {

        }
    }
}
