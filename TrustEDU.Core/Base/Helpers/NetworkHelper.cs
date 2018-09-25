using System.IO;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Base.Helpers
{
    public static class NetworkHelper
    {
        public static byte[] GetHashData(this IVerifiable verifiable)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                verifiable.SerializeUnsigned(writer);
                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}