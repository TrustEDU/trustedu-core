using System;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Json;
using TrustEDU.Core.Base.Types;

namespace TrustEDU.Core.Models.Wallets.TERC1
{
    internal class TERC1Account : WalletAccount
    {
        private readonly TERC1Wallet wallet;
        private readonly string tercKey;
        private KeyPair key;
        public JObject Extra;

        public bool Decrypted => tercKey == null || key != null;
        public override bool HasKey => tercKey != null;

        public TERC1Account(TERC1Wallet wallet, UInt160 scriptHash, string tercKey = null)
            : base(scriptHash)
        {
            this.wallet = wallet;
            this.tercKey = tercKey;
        }

        public TERC1Account(TERC1Wallet wallet, UInt160 scriptHash, KeyPair key, string password)
            : this(wallet, scriptHash, key.Export(password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P))
        {
            this.key = key;
        }

        public static TERC1Account FromJson(JObject json, TERC1Wallet wallet)
        {
            return new TERC1Account(wallet, json["address"].AsString().ToScriptHash(), json["key"]?.AsString())
            {
                Label = json["label"]?.AsString(),
                IsDefault = json["isDefault"].AsBoolean(),
                Lock = json["lock"].AsBoolean(),
                Contract = TERC1Contract.FromJson(json["contract"]),
                Extra = json["extra"]
            };
        }

        public override KeyPair GetKey()
        {
            if (tercKey == null) return null;
            if (key == null)
            {
                key = wallet.DecryptKey(tercKey);
            }
            return key;
        }

        public KeyPair GetKey(string password)
        {
            if (tercKey == null) return null;
            if (key == null)
            {
                key = new KeyPair(Wallet.GetPrivateKeyFromNEP2(tercKey, password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P));
            }
            return key;
        }

        public JObject ToJson()
        {
            JObject account = new JObject();
            account["address"] = ScriptHash.ToAddress();
            account["label"] = Label;
            account["isDefault"] = IsDefault;
            account["lock"] = Lock;
            account["key"] = tercKey;
            account["contract"] = ((TERC1Contract)Contract)?.ToJson();
            account["extra"] = Extra;
            return account;
        }

        public bool VerifyPassword(string password)
        {
            try
            {
                Wallet.GetPrivateKeyFromNEP2(tercKey, password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
