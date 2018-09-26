using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TrustEDU.Core.Models.Transactions;
using TrustEDU.Core.Base.Types;

namespace TrustEDU.Core
{
    public class Configs
    {
        public uint Magic { get; private set; }
        public byte AddressVersion { get; private set; }
        public string[] StandbyValidators { get; private set; }
        public string[] SeedList { get; private set; }
        public IReadOnlyDictionary<TransactionType, Fixed8> SystemFee { get; private set; }
        public uint SecondsPerBlock { get; private set; }

        public static Configs Default { get; private set; }

        static Configs()
        {
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("protocol.json").Build().GetSection("Protocol");
            Default = new Configs(section);
        }

        public Configs(IConfigurationSection section)
        {
            this.Magic = uint.Parse(section.GetSection("Magic").Value);
            this.AddressVersion = byte.Parse(section.GetSection("AddressVersion").Value);
            this.StandbyValidators = section.GetSection("StandbyValidators").GetChildren().Select(p => p.Value).ToArray();
            this.SeedList = section.GetSection("SeedList").GetChildren().Select(p => p.Value).ToArray();
            this.SystemFee = section.GetSection("SystemFee").GetChildren().ToDictionary(p => (TransactionType)Enum.Parse(typeof(TransactionType), p.Key, true), p => Fixed8.Parse(p.Value));
            this.SecondsPerBlock = GetValueOrDefault(section.GetSection("SecondsPerBlock"), 15u, p => uint.Parse(p));
        }

        public T GetValueOrDefault<T>(IConfigurationSection section, T defaultValue, Func<string, T> selector)
        {
            return section.Value == null ? defaultValue : selector(section.Value);
        }
    }
}
