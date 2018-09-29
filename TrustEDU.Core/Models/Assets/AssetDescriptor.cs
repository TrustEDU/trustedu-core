using System;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.SmartContract;
using TrustEDU.VM.Base;
using TrustEDU.VM.Runtime;

namespace TrustEDU.Core.Models.Assets
{
    public class AssetDescriptor
    {
        public UIntBase AssetId;
        public string AssetName;
        public byte Decimals;

        public AssetDescriptor(UIntBase assetId)
        {
            if (assetId is UInt160 assetId160)
            {
                byte[] script;
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    sb.EmitAppCall(assetId160, "decimals");
                    sb.EmitAppCall(assetId160, "name");
                    script = sb.ToArray();
                }
                ApplicationEngine engine = ApplicationEngine.Run(script);
                if (engine.State.HasFlag(VMState.FAULT)) throw new ArgumentException();
                this.AssetId = assetId;
                this.AssetName = engine.ResultStack.Pop().GetString();
                this.Decimals = (byte)engine.ResultStack.Pop().GetBigInteger();
            }
            else
            {
                AssetState state = Blockchain.Singleton.Store.GetAssets()[(UInt256)assetId];
                this.AssetId = state.AssetId;
                this.AssetName = state.GetName();
                this.Decimals = state.Precision;
            }
        }

        public override string ToString()
        {
            return AssetName;
        }
    }
}
