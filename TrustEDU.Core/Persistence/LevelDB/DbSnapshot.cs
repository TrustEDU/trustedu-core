using TrustEDU.Core.Base.Caching;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Cryptography.ECC;
using TrustEDU.Core.Models.Assets;
using TrustEDU.Core.Models.Coins;
using TrustEDU.Core.Models.Ledger;
using TrustEDU.Core.Models.LevelDB;
using TrustEDU.Core.Models.SmartContract;
using TrustEDU.Core.Models.Transactions;
using TrustEDU.Core.Models.Wallets;
using LvSnapshot = TrustEDU.Core.Models.LevelDB.Snapshot;

namespace TrustEDU.Core.Persistence.LevelDB
{
    internal class DbSnapshot : Snapshot
    {
        private readonly Database db;
        private readonly LvSnapshot snapshot;
        private readonly WriteBatch batch;

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

        public DbSnapshot(Database db)
        {
            this.db = db;
            this.snapshot = db.GetSnapshot();
            this.batch = new WriteBatch();
            ReadOptions options = new ReadOptions { FillCache = false, Snapshot = snapshot };
            Blocks = new DbCache<UInt256, BlockState>(db, options, batch, Prefixes.DATA_Block);
            Transactions = new DbCache<UInt256, TransactionState>(db, options, batch, Prefixes.DATA_Transaction);
            Accounts = new DbCache<UInt160, AccountState>(db, options, batch, Prefixes.ST_Account);
            UnspentCoins = new DbCache<UInt256, UnspentCoinState>(db, options, batch, Prefixes.ST_Coin);
            SpentCoins = new DbCache<UInt256, SpentCoinState>(db, options, batch, Prefixes.ST_SpentCoin);
            Validators = new DbCache<ECPoint, ValidatorState>(db, options, batch, Prefixes.ST_Validator);
            Assets = new DbCache<UInt256, AssetState>(db, options, batch, Prefixes.ST_Asset);
            Contracts = new DbCache<UInt160, ContractState>(db, options, batch, Prefixes.ST_Contract);
            Storages = new DbCache<StorageKey, StorageItem>(db, options, batch, Prefixes.ST_Storage);
            HeaderHashList = new DbCache<UInt32Wrapper, HeaderHashList>(db, options, batch, Prefixes.IX_HeaderHashList);
            ValidatorsCount = new DbMetaDataCache<ValidatorsCountState>(db, options, batch, Prefixes.IX_ValidatorsCount);
            BlockHashIndex = new DbMetaDataCache<HashIndexState>(db, options, batch, Prefixes.IX_CurrentBlock);
            HeaderHashIndex = new DbMetaDataCache<HashIndexState>(db, options, batch, Prefixes.IX_CurrentHeader);
        }

        public override void Commit()
        {
            base.Commit();
            db.Write(WriteOptions.Default, batch);
        }

        public override void Dispose()
        {
            snapshot.Dispose();
        }
    }
}
