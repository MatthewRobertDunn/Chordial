using NaCl.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography.Primitives
{
    public static class SymmetricPrimitives
    {
        public static byte[] Decrypt(this byte[] symmetricKey, byte[] nonce, byte[] additionalData, byte[] cipherText)
        {
            var chacha = new ChaCha20Poly1305(symmetricKey);
            return chacha.Decrypt(cipherText, additionalData, nonce);
        }
    }
}
