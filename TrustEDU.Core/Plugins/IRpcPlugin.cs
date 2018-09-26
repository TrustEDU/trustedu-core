using Microsoft.AspNetCore.Http;
using TrustEDU.Core.Base.Json;

namespace TrustEDU.Core.Plugins
{
    public interface IRpcPlugin
    {
        JObject OnProcess(HttpContext context, string method, JArray _params);
    }
}
