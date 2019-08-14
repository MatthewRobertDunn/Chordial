using Org.BouncyCastle.Crypto.Parameters;
using Hive.Cryptography.Primitives;
using System;

namespace Hive.Cryptography
{
    /// <summary>
    /// High level API to Hive sealed messages.
    /// </summary>
    public class SealedMessages
    {
        private readonly ECPrivateKeyParameters privateKey;
        private readonly ECPublicKeyParameters publicKey;

        //session data that is unique to the above keypair
        private readonly byte[] sharedKey;
        private readonly byte[] symmetricNonce;

        //Info text that is passed into the Hkdf key derive algorithm
        private const string symmetricKeyDeriveInfoText = "handshake data";
        private const string nonceKeyDeriveInfoText = "nonce data";

        /// <summary>
        /// Initializes a new instance of SealedMessages for a given keypair
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="publicKey"></param>
        public SealedMessages(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;

            //Create keys which are unique to this private/public pair to save cpu when decrypting many messages.
            //Get the secret diffie-hellman key.
            this.sharedKey = privateKey.GetSharedKey(publicKey);

            //nonce for chacha.
            this.symmetricNonce = this.sharedKey.DeriveKey(nonceKeyDeriveInfoText.ToUTF8Bytes(), 12);
        }
            
        /// <summary>
        /// Decrypts a hiv compact form message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public DecryptedAeadMessage DecryptMessage(string message)
        {
            //Decodes hive aead message into byte arrays, perform some validation
            var aeadMessage = DecodeHiveCompactAeadMessage(message);

            //Derive the 256bit chachapoly key from the sharedkey and the message IV.
            //this is unique per message.
            var symmetricKey = this.sharedKey.DeriveKey(aeadMessage.InitializationVector, 32);

            //Decrypt the ciphertext and verify the additional data section using the symmetric key and nonce.
            var plainText = symmetricKey.Decrypt(this.symmetricNonce, aeadMessage.AdditionalData, aeadMessage.CipherText);

            return new DecryptedAeadMessage()
            {
                AdditionalData = aeadMessage.AdditionalData.FromUTF8(),
                ProtectedData = plainText.FromUTF8(),
            };
        }

        /// <summary>
        /// Decodes a hive compact form (iv.extraData.cipherText) Aead message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private AeadMessage DecodeHiveCompactAeadMessage(string message)
        {
            var textParts = message.Split('.');
            if (textParts.Length != 3)
                throw new ArgumentException("Hive compact aead message has the wrong number of parts.");

            var result = new AeadMessage()
            {
                InitializationVector = Convert.FromBase64String(textParts[0]),
                AdditionalData = Convert.FromBase64String(textParts[1]),
                CipherText = Convert.FromBase64String(textParts[2])
            };

            if (result.InitializationVector.Length != 16)
                throw new ArgumentException("Initialization vector is the wrong length");

            return result;
        }
    }
}
