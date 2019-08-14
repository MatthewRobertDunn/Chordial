using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography.Certificates
{
    public class SigningCertWithPrivateKey
    {
        public SigningCertWithPrivateKey(X509Certificate certificate, Ed448PrivateKeyParameters privateKey)
        {
            this.PrivateKey = privateKey;
            this.Certificate = certificate;
            this.PublicKey = (Ed448PublicKeyParameters)Certificate.GetPublicKey();
        }
        public Ed448PrivateKeyParameters PrivateKey { get; }

        public Ed448PublicKeyParameters PublicKey { get; }
        public X509Certificate Certificate { get; }
    }
}
