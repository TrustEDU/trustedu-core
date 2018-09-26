using System;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Transactions;

namespace TrustEDU.Core.Models.Wallets
{
    public class WalletTransactionEventArgs : EventArgs
    {
        public Transaction Transaction;
        public UInt160[] RelatedAccounts;
        public uint? Height;
        public uint Time;
    }
}