namespace TrustEDU.Core.Persistence.LevelDB
{
    internal static class Prefixes
    {
        public const byte DataBlock = 0x01;
        public const byte DataTransaction = 0x02;

        public const byte StorageAccount = 0x40;
        public const byte StorageCoin = 0x44;
        public const byte StorageSpentCoin = 0x45;
        public const byte StorageValidator = 0x48;
        public const byte StorageAsset = 0x4c;
        public const byte StorageContract = 0x50;
        public const byte StorageCommon = 0x70;

        public const byte IndexHeaderHashList = 0x80;
        public const byte IndexValidatorsCount = 0x90;
        public const byte IndexCurrentBlock = 0xc0;
        public const byte IndexCurrentHeader = 0xc1;

        public const byte SysVersion = 0xf0;
    }
}
