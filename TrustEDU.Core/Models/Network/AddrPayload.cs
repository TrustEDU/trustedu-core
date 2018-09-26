using System;
using System.IO;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Models.Common;

namespace TrustEDU.Core.Models.Network
{
    public class AddrPayload : ISerializable
    {
        public const int MaxCountToSend = 200;

        public NetworkAddressWithTime[] AddressList;

        public int Size => AddressList.GetVarSize();

        public static AddrPayload Create(params NetworkAddressWithTime[] addresses)
        {
            return new AddrPayload
            {
                AddressList = addresses
            };
        }

        void ISerializable.Deserialize(BinaryReader reader)
        {
            AddressList = reader.ReadSerializableArray<NetworkAddressWithTime>(MaxCountToSend);
            if (AddressList.Length == 0)
                throw new FormatException();
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(AddressList);
        }
    }
}
