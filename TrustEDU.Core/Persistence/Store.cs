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
    public abstract class Store : IPersistence
    {
        CacheItem<UInt256, BlockState> IPersistence.Blocks => GetBlocks();
        CacheItem<UInt256, TransactionState> IPersistence.Transactions => GetTransactions();
        CacheItem<UInt160, AccountState> IPersistence.Accounts => GetAccounts();
        CacheItem<UInt256, UnspentCoinState> IPersistence.UnspentCoins => GetUnspentCoins();
        CacheItem<UInt256, SpentCoinState> IPersistence.SpentCoins => GetSpentCoins();
        CacheItem<ECPoint, ValidatorState> IPersistence.Validators => GetValidators();
        CacheItem<UInt256, AssetState> IPersistence.Assets => GetAssets();
        CacheItem<UInt160, ContractState> IPersistence.Contracts => GetContracts();
        CacheItem<StorageKey, StorageItem> IPersistence.Storages => GetStorages();
        CacheItem<UInt32Wrapper, HeaderHashList> IPersistence.HeaderHashList => GetHeaderHashList();
        MetaDataCache<ValidatorsCountState> IPersistence.ValidatorsCount => GetValidatorsCount();
        MetaDataCache<HashIndexState> IPersistence.BlockHashIndex => GetBlockHashIndex();
        MetaDataCache<HashIndexState> IPersistence.HeaderHashIndex => GetHeaderHashIndex();

        public abstract CacheItem<UInt256, BlockState> GetBlocks();
        public abstract CacheItem<UInt256, TransactionState> GetTransactions();
        public abstract CacheItem<UInt160, AccountState> GetAccounts();
        public abstract CacheItem<UInt256, UnspentCoinState> GetUnspentCoins();
        public abstract CacheItem<UInt256, SpentCoinState> GetSpentCoins();
        public abstract CacheItem<ECPoint, ValidatorState> GetValidators();
        public abstract CacheItem<UInt256, AssetState> GetAssets();
        public abstract CacheItem<UInt160, ContractState> GetContracts();
        public abstract CacheItem<StorageKey, StorageItem> GetStorages();
        public abstract CacheItem<UInt32Wrapper, HeaderHashList> GetHeaderHashList();
        public abstract MetaDataCache<ValidatorsCountState> GetValidatorsCount();
        public abstract MetaDataCache<HashIndexState> GetBlockHashIndex();
        public abstract MetaDataCache<HashIndexState> GetHeaderHashIndex();

        public abstract Snapshot GetSnapshot();
    }
}
