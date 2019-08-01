using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hive.Cryptography.Primitives
{
    public static class EccPrimitives
    {
        /// <summary>
        /// Returns a DH shared key
        /// </summary>
        /// <param name="priv">The private key</param>
        /// <param name="pub">The public key</param>
        /// <returns></returns>
        public static byte[] GetSharedKey(this ECPrivateKeyParameters priv, ECPublicKeyParameters pub)
        {
            var agreement = new ECDHBasicAgreement();
            agreement.Init(priv);
            return agreement.CalculateAgreement(pub).ToByteArray();
        }

        /// <summary>
        /// Derives a key using a given shared salt and a DH shared key.
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="salt"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] DeriveKey(this byte[] secret, byte[] salt, int length)
        {
            var key = secret.Concat(salt).ToArray();
            var shake = new ShakeDigest(256);
            shake.BlockUpdate(key, 0, key.Length);
            var derivedKey = new byte[length];
            shake.DoFinal(derivedKey, 0, length);
            return derivedKey;
        }


      /*  public static byte[] DeriveKey2(this byte[] sharedKey, byte[] salt, string info, int length)
        {
            var digest = new ShakeDigest(256);
        }*/


        /// <summary>
        /// Converts a private ecc key to a public one.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static ECPublicKeyParameters ToPublicKey(this ECPrivateKeyParameters p)
        {
            var q = p.Parameters.G.Multiply(p.D);
            return new ECPublicKeyParameters(q, p.Parameters);
        }
    }
}
