using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace TrustEDU.Core.Base.Helpers
{
    public static class PluginHelper
    {
        public static IConfigurationSection GetConfiguration(this Assembly assembly)
        {
            var path = Path.Combine("Plugins", assembly.GetName().Name, "config.json");
            return new ConfigurationBuilder().AddJsonFile(path).Build().GetSection("Plugin");
        }
    }
}