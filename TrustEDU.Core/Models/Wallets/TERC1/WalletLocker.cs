using System;

namespace TrustEDU.Core.Models.Wallets.TERC1
{
    internal class WalletLocker : IDisposable
    {
        private TERC1Wallet wallet;

        public WalletLocker(TERC1Wallet wallet)
        {
            this.wallet = wallet;
        }

        public void Dispose()
        {
            wallet.Lock();
        }
    }
}
