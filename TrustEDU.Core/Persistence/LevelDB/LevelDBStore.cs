using System;
using System.Reflection;
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

namespace TrustEDU.Core.Persistence.LevelDB
{
    public class LevelDBStore : Store, IDisposable
    {
        private readonly Database db;

        public LevelDBStore(string path)
        {
            this.db = Database.Open(path, new Options { CreateIfMissing = true });
            if (db.TryGet(ReadOptions.Default, SliceBuilder.Begin(Prefixes.SysVersion), out Slice value) && Version.TryParse(value.ToString(), out Version version) && version >= Version.Parse("2.9.0"))
                return;
            WriteBatch batch = new WriteBatch();
            ReadOptions options = new ReadOptions { FillCache = false };
            using (Iterator it = db.NewIterator(options))
            {
                for (it.SeekToFirst(); it.Valid(); it.Next())
                {
                    batch.Delete(it.Key());
                }
            }
            db.Put(WriteOptions.Default, SliceBuilder.Begin(Prefixes.SysVersion), Assembly.GetExecutingAssembly().GetName().Version.ToString());
            db.Write(WriteOptions.Default, batch);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public override CacheItem<UInt160, AccountState> GetAccounts()
        {
            return new DbCache<UInt160, AccountState>(db, null, null, Prefixes.StorageAccount);
        }

        public override CacheItem<UInt256, AssetState> GetAssets()
        {
            return new DbCache<UInt256, AssetState>(db, null, null, Prefixes.StorageAsset);
        }

        public override CacheItem<UInt256, BlockState> GetBlocks()
        {
            return new DbCache<UInt256, BlockState>(db, null, null, Prefixes.DataBlock);
        }

        public override CacheItem<UInt160, ContractState> GetContracts()
        {
            return new DbCache<UInt160, ContractState>(db, null, null, Prefixes.StorageContract);
        }

        public override Snapshot GetSnapshot()
        {
            return new DbSnapshot(db);
        }

        public override CacheItem<UInt256, SpentCoinState> GetSpentCoins()
        {
            return new DbCache<UInt256, SpentCoinState>(db, null, null, Prefixes.StorageSpentCoin);
        }

        public override CacheItem<StorageKey, StorageItem> GetStorages()
        {
            return new DbCache<StorageKey, StorageItem>(db, null, null, Prefixes.StorageCommon);
        }

        public override CacheItem<UInt256, TransactionState> GetTransactions()
        {
            return new DbCache<UInt256, TransactionState>(db, null, null, Prefixes.DataTransaction);
        }

        public override CacheItem<UInt256, UnspentCoinState> GetUnspentCoins()
        {
            return new DbCache<UInt256, UnspentCoinState>(db, null, null, Prefixes.StorageCoin);
        }

        public override CacheItem<ECPoint, ValidatorState> GetValidators()
        {
            return new DbCache<ECPoint, ValidatorState>(db, null, null, Prefixes.StorageValidator);
        }

        public override CacheItem<UInt32Wrapper, HeaderHashList> GetHeaderHashList()
        {
            return new DbCache<UInt32Wrapper, HeaderHashList>(db, null, null, Prefixes.IndexHeaderHashList);
        }

        public override MetaDataCache<ValidatorsCountState> GetValidatorsCount()
        {
            return new DbMetaDataCache<ValidatorsCountState>(db, null, null, Prefixes.IndexValidatorsCount);
        }

        public override MetaDataCache<HashIndexState> GetBlockHashIndex()
        {
            return new DbMetaDataCache<HashIndexState>(db, null, null, Prefixes.IndexCurrentBlock);
        }

        public override MetaDataCache<HashIndexState> GetHeaderHashIndex()
        {
            return new DbMetaDataCache<HashIndexState>(db, null, null, Prefixes.IndexCurrentHeader);
        }
    }
}
