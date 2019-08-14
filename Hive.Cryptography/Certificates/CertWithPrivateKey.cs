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
        public CertWithPrivateKey(X509Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            this.PrivateKey = privateKey;
            this.Certificate = certificate;
            this.PublicKey = (AsymmetricKeyParameter)this.Certificate.GetPublicKey();
        }
        public AsymmetricKeyParameter PrivateKey { get; }

        public AsymmetricKeyParameter PublicKey { get; }
        public X509Certificate Certificate { get; }
    }
}
