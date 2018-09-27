namespace TrustEDU.Core.Models.Wallets.Accounts
{
    internal class Account
    {
        public byte[] PrivateKeyEncrypted { get; set; }
        public byte[] PublicKeyHash { get; set; }
    }
}