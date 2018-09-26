using System;
using TrustEDU.Core.Base.Caching;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Cryptography.ECC;
using TrustEDU.Core.Models.Assets;
using TrustEDU.Core.Models.Coins;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.SmartContract;
using TrustEDU.Core.Models.Transactions;
using TrustEDU.Core.Models.Wallets;

namespace TrustEDU.Core.Persistence
{
    internal class CloneSnapshot : Snapshot
    {
        public override CacheItem<UInt256, BlockState> Blocks { get; }
        public override CacheItem<UInt256, TransactionState> Transactions { get; }
        public override CacheItem<UInt160, AccountState> Accounts { get; }
        public override CacheItem<UInt256, UnspentCoinState> UnspentCoins { get; }
        public override CacheItem<UInt256, SpentCoinState> SpentCoins { get; }
        public override CacheItem<ECPoint, ValidatorState> Validators { get; }
        public override CacheItem<UInt256, AssetState> Assets { get; }
        public override CacheItem<UInt160, ContractState> Contracts { get; }
        public override CacheItem<StorageKey, StorageItem> Storages { get; }
        public override CacheItem<UInt32Wrapper, HeaderHashList> HeaderHashList { get; }
        public override MetaDataCache<ValidatorsCountState> ValidatorsCount { get; }
        public override MetaDataCache<HashIndexState> BlockHashIndex { get; }
        public override MetaDataCache<HashIndexState> HeaderHashIndex { get; }

        public CloneSnapshot(Snapshot snapshot)
        {
            this.PersistingBlock = snapshot.PersistingBlock;
            this.Blocks = snapshot.Blocks.CreateSnapshot();
            this.Transactions = snapshot.Transactions.CreateSnapshot();
            this.Accounts = snapshot.Accounts.CreateSnapshot();
            this.UnspentCoins = snapshot.UnspentCoins.CreateSnapshot();
            this.SpentCoins = snapshot.SpentCoins.CreateSnapshot();
            this.Validators = snapshot.Validators.CreateSnapshot();
            this.Assets = snapshot.Assets.CreateSnapshot();
            this.Contracts = snapshot.Contracts.CreateSnapshot();
            this.Storages = snapshot.Storages.CreateSnapshot();
            this.HeaderHashList = snapshot.HeaderHashList.CreateSnapshot();
            this.ValidatorsCount = snapshot.ValidatorsCount.CreateSnapshot();
            this.BlockHashIndex = snapshot.BlockHashIndex.CreateSnapshot();
            this.HeaderHashIndex = snapshot.HeaderHashIndex.CreateSnapshot();
        }
    }
}
