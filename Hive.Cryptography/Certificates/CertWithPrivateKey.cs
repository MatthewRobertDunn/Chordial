using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography.Certificates
{
    public class CertWithPrivateKey
    {
        public CertWithPrivateKey(X509Certificate certificate, ECPrivateKeyParameters privateKey)
        {
            this.PrivateKey = privateKey;
            this.Certificate = certificate;
            this.PublicKey = (ECPublicKeyParameters)this.Certificate.GetPublicKey();
        }
        public ECPrivateKeyParameters PrivateKey { get; }

        public ECPublicKeyParameters PublicKey { get; }
        public X509Certificate Certificate { get; }
    }
}
