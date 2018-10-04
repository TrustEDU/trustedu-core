using System.Collections;
using System.Linq;
using TrustEDU.Core.Base.Helpers;

namespace TrustEDU.Core.Cryptography
{
    public class BloomFilter
    {
        private readonly uint[] _seeds;
        private readonly BitArray _bits;

        public int K => _seeds.Length;

        public int M => _bits.Length;

        public uint Tweak { get; private set; }

        public BloomFilter(int m, int k, uint nTweak, byte[] elements = null)
        {
            this._seeds = Enumerable.Range(0, k).Select(p => (uint)p * 0xFBA4C795 + nTweak).ToArray();
            this._bits = elements == null ? new BitArray(m) : new BitArray(elements);
            this._bits.Length = m;
            this.Tweak = nTweak;
        }

        public void Add(byte[] element)
        {
            foreach (uint i in _seeds.AsParallel().Select(element.Murmur32))
                _bits.Set((int)(i % (uint)_bits.Length), true);
        }

        public bool Check(byte[] element)
        {
            foreach (uint i in _seeds.AsParallel().Select(element.Murmur32))
                if (!_bits.Get((int)(i % (uint)_bits.Length)))
                    return false;
            return true;
        }

        public void GetBits(byte[] newBits)
        {
            _bits.CopyTo(newBits, 0);
        }
    }
}
