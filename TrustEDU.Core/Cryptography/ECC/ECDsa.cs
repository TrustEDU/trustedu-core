using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using TrustEDU.Core.Base.Helpers;

namespace TrustEDU.Core.Cryptography.ECC
{
    public class ECDsa
    {
        private readonly byte[] _privateKey;
        private readonly ECPoint _publicKey;
        private readonly ECCurve _curve;

        public ECDsa(byte[] privateKey, ECCurve curve)
            : this(curve.G * privateKey)
        {
            this._privateKey = privateKey;
        }

        public ECDsa(ECPoint publicKey)
        {
            this._publicKey = publicKey;
            this._curve = publicKey.Curve;
        }

        private BigInteger CalculateE(BigInteger n, byte[] message)
        {
            int messageBitLength = message.Length * 8;
            BigInteger trunc = new BigInteger(message.Reverse().Concat(new byte[1]).ToArray());
            if (n.GetBitLength() < messageBitLength)
            {
                trunc >>= messageBitLength - n.GetBitLength();
            }
            return trunc;
        }

        public BigInteger[] GenerateSignature(byte[] message)
        {
            if (_privateKey == null) throw new InvalidOperationException();
            BigInteger e = CalculateE(_curve.N, message);
            BigInteger d = new BigInteger(_privateKey.Reverse().Concat(new byte[1]).ToArray());
            BigInteger r, s;
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                do
                {
                    BigInteger k;
                    do
                    {
                        do
                        {
                            k = rng.NextBigInteger(_curve.N.GetBitLength());
                        }
                        while (k.Sign == 0 || k.CompareTo(_curve.N) >= 0);
                        ECPoint p = ECPoint.Multiply(_curve.G, k);
                        BigInteger x = p.X.Value;
                        r = x.Mod(_curve.N);
                    }
                    while (r.Sign == 0);
                    s = (k.ModInverse(_curve.N) * (e + d * r)).Mod(_curve.N);
                    if (s > _curve.N / 2)
                    {
                        s = _curve.N - s;
                    }
                }
                while (s.Sign == 0);
            }
            return new BigInteger[] { r, s };
        }

        private static ECPoint SumOfTwoMultiplies(ECPoint P, BigInteger k, ECPoint Q, BigInteger l)
        {
            int m = Math.Max(k.GetBitLength(), l.GetBitLength());
            ECPoint Z = P + Q;
            ECPoint R = P.Curve.Infinity;
            for (int i = m - 1; i >= 0; --i)
            {
                R = R.Twice();
                if (k.TestBit(i))
                {
                    if (l.TestBit(i))
                        R = R + Z;
                    else
                        R = R + P;
                }
                else
                {
                    if (l.TestBit(i))
                        R = R + Q;
                }
            }
            return R;
        }

        public bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
        {
            if (r.Sign < 1 || s.Sign < 1 || r.CompareTo(_curve.N) >= 0 || s.CompareTo(_curve.N) >= 0)
                return false;
            BigInteger e = CalculateE(_curve.N, message);
            BigInteger c = s.ModInverse(_curve.N);
            BigInteger u1 = (e * c).Mod(_curve.N);
            BigInteger u2 = (r * c).Mod(_curve.N);
            ECPoint point = SumOfTwoMultiplies(_curve.G, u1, _publicKey, u2);
            BigInteger v = point.X.Value.Mod(_curve.N);
            return v.Equals(r);
        }
    }
}
