﻿using System;
using System.Linq;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Cryptography;
using TrustEDU.Core.Models.Common;
using TrustEDU.Core.Models.Wallets;

namespace TrustEDU.Core.Base.Helpers
{
    public static class WalletHelper
    {
        public static byte[] Sign(this IVerifiable verifiable, KeyPair key)
        {
            return Crypto.Default.Sign(verifiable.GetHashData(), key.PrivateKey, key.PublicKey.EncodePoint(false).Skip(1).ToArray());
        }

        public static string ToAddress(this UInt160 scriptHash)
        {
            byte[] data = new byte[21];
            data[0] = Configs.Default.AddressVersion;
            Buffer.BlockCopy(scriptHash.ToArray(), 0, data, 1, 20);
            return data.Base58CheckEncode();
        }

        public static UInt160 ToScriptHash(this string address)
        {
            byte[] data = address.Base58CheckDecode();
            if (data.Length != 21)
                throw new FormatException();
            if (data[0] != Configs.Default.AddressVersion)
                throw new FormatException();
            return new UInt160(data.Skip(1).ToArray());
        }
    }
}
