using Org.BouncyCastle.Crypto;
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
        }
        public AsymmetricKeyParameter PrivateKey { get; }
        public X509Certificate Certificate { get; }
    }
}
