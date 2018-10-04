using System;
using System.Collections.Generic;
using System.Text;
using TrustEDU.Core.Repositories;

namespace TrustEDU.Core.Persistence
{
    internal interface IPersistenceFactory
    {
        ILedgerRepository GetLedgerRepository();
    }
}
