using System.IO;
using System.Linq;
using TrustEDU.Core.Base.Caching;
using TrustEDU.Core.Base.Helpers;
using TrustEDU.Core.Base.Json;
using TrustEDU.Core.Base.Types;
using TrustEDU.Core.Models.Transactions;

namespace TrustEDU.Core.Models.Ledger
{
    public class TrimmedBlock : BlockBase
    {
        public UInt256[] Hashes;

        public bool IsBlock => Hashes.Length > 0;

        public Block GetBlock(CacheItem<UInt256, TransactionState> cache)
        {
            return new Block
            {
                Version = Version,
                PrevHash = PrevHash,
                MerkleRoot = MerkleRoot,
                Timestamp = Timestamp,
                Index = Index,
                ConsensusData = ConsensusData,
                NextConsensus = NextConsensus,
                Witness = Witness,
                Transactions = Hashes.Select(p => cache[p].Transaction).ToArray()
            };
        }

        private Header _header = null;
        public Header Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new Header
                    {
                        Version = Version,
                        PrevHash = PrevHash,
                        MerkleRoot = MerkleRoot,
                        Timestamp = Timestamp,
                        Index = Index,
                        ConsensusData = ConsensusData,
                        NextConsensus = NextConsensus,
                        Witness = Witness
                    };
                }
                return _header;
            }
        }

        public override int Size => base.Size + Hashes.GetVarSize();

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Hashes = reader.ReadSerializableArray<UInt256>();
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Hashes);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["hashes"] = Hashes.Select(p => (JObject)p.ToString()).ToArray();
            return json;
        }
    }
}
