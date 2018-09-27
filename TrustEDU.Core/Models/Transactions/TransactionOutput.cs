﻿using System;
using System.IO;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Json;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Common;
using TrustEDU.VM.Base;

namespace TrustEDU.Core.Models.Transactions
{
    public class TransactionOutput : IInteropContract, ISerializable
    {
        public UInt256 AssetId;
        public Fixed8 Value;
        public UInt160 ScriptHash;

        public int Size => AssetId.Size + Value.Size + ScriptHash.Size;

        void ISerializable.Deserialize(BinaryReader reader)
        {
            this.AssetId = reader.ReadSerializable<UInt256>();
            this.Value = reader.ReadSerializable<Fixed8>();
            if (Value <= Fixed8.Zero) throw new FormatException();
            this.ScriptHash = reader.ReadSerializable<UInt160>();
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(AssetId);
            writer.Write(Value);
            writer.Write(ScriptHash);
        }

        public JObject ToJson(ushort index)
        {
            JObject json = new JObject();
            json["n"] = index;
            json["asset"] = AssetId.ToString();
            json["value"] = Value.ToString();
            json["address"] = ScriptHash.ToAddress();
            return json;
        }
    }
}
