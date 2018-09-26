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
    public interface IPersistence
    {
        CacheItem<UInt256, BlockState> Blocks { get; }
        CacheItem<UInt256, TransactionState> Transactions { get; }
        CacheItem<UInt160, AccountState> Accounts { get; }
        CacheItem<UInt256, UnspentCoinState> UnspentCoins { get; }
        CacheItem<UInt256, SpentCoinState> SpentCoins { get; }
        CacheItem<ECPoint, ValidatorState> Validators { get; }
        CacheItem<UInt256, AssetState> Assets { get; }
        CacheItem<UInt160, ContractState> Contracts { get; }
        CacheItem<StorageKey, StorageItem> Storages { get; }
        CacheItem<UInt32Wrapper, HeaderHashList> HeaderHashList { get; }
        MetaDataCache<ValidatorsCountState> ValidatorsCount { get; }
        MetaDataCache<HashIndexState> BlockHashIndex { get; }
        MetaDataCache<HashIndexState> HeaderHashIndex { get; }
    }
}
