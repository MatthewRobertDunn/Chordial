using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography.Primitives
{
    public static class DigestPrimitives
    {
        public static byte[] ShakeHash(this byte[] source, int length)
        {
            var shake = new ShakeDigest(256);
            shake.BlockUpdate(source, 0, source.Length);
            var derivedKey = new byte[length];
            shake.DoFinal(derivedKey, 0, length);
            return derivedKey;
        }

        public static byte[] SHAThree256(this byte[] source)
        {
            var sha3 = new Sha3Digest(256);
            sha3.BlockUpdate(source, 0, source.Length);
            var derivedKey = new byte[32];
            sha3.DoFinal(derivedKey, 0);
            return derivedKey;
        }


        public static string ToBase64(this byte[] source)
        {
            return Convert.ToBase64String(source);
        }
    }
}
