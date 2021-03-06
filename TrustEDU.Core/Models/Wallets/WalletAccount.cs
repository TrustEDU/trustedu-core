﻿using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.SmartContract;

namespace TrustEDU.Core.Models.Wallets
{
    public abstract class WalletAccount
    {
        public readonly UInt160 ScriptHash;
        public string Label;
        public bool IsDefault;
        public bool Lock;
        public Contract Contract;

        public string Address => ScriptHash.ToAddress();
        public abstract bool HasKey { get; }
        public bool WatchOnly => Contract == null;

        public abstract KeyPair GetKey();

        protected WalletAccount(UInt160 scriptHash)
        {
            this.ScriptHash = scriptHash;
        }
    }
}
