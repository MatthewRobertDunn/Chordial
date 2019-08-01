using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="sharedKey"></param>
        /// <param name="saltText"></param>
        /// <param name="info"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] DeriveKey(this byte[] sharedKey, byte[] salt, string info, int length)
        {
            var keygen = new HkdfBytesGenerator(new Sha3Digest(256));
            keygen.Init(new HkdfParameters(sharedKey, salt, ASCIIEncoding.Default.GetBytes(info)));
            var derivedKey = new byte[length];
            keygen.GenerateBytes(derivedKey, 0, length);
            return derivedKey;
        }


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
