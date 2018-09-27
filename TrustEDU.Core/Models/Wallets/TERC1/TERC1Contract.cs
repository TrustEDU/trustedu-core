using System.Linq;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Json;
using TrustEDU.Core.Models.SmartContract;

namespace TrustEDU.Core.Models.Wallets.TERC1
{
    internal class TERC1Contract : Contract
    {
        public string[] ParameterNames;
        public bool Deployed;

        public static TERC1Contract FromJson(JObject json)
        {
            if (json == null) return null;
            return new TERC1Contract
            {
                Script = json["script"].AsString().HexToBytes(),
                ParameterList = ((JArray)json["parameters"]).Select(p => p["type"].AsEnum<ContractParameterType>()).ToArray(),
                ParameterNames = ((JArray)json["parameters"]).Select(p => p["name"].AsString()).ToArray(),
                Deployed = json["deployed"].AsBoolean()
            };
        }

        public JObject ToJson()
        {
            JObject contract = new JObject();
            contract["script"] = Script.ToHexString();
            contract["parameters"] = new JArray(ParameterList.Zip(ParameterNames, (type, name) =>
            {
                JObject parameter = new JObject();
                parameter["name"] = name;
                parameter["type"] = type;
                return parameter;
            }));
            contract["deployed"] = Deployed;
            return contract;
        }
    }
}
