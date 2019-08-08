using Hive.Cryptography.Primitives;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography.Certificates
{
    public class CertificateStore : ICertificateStore
    {
        private CertWithPrivateKey rootCert;
        private CertWithPrivateKey transportCert;
        private CertWithPrivateKey channelCert;
        private CertWithPrivateKey privateCert;

        /// <summary>
        /// Creates a brand new set of hive keys for use.
        /// </summary>
        public void Generate()
        {
            rootCert = CertPrimitives.CreateRootCertificate();
            transportCert = CertPrimitives.CreateChildCertificate(rootCert, "CN=Transport", "ECDSA");
            channelCert = CertPrimitives.CreateChildCertificate(rootCert, "CN=Channel", "ECDSA");
            privateCert = CertPrimitives.CreateChildCertificate(rootCert, "CN=Private", "ECDH");

            //Todo: Save to disk
        }

        public bool Load()
        {
            //todo: load
            throw new NotImplementedException();
        }


        public byte[] HiveAddress => throw new NotImplementedException();

        public CertWithPrivateKey Transport => transportCert;

        public CertWithPrivateKey Channel => channelCert;

        public CertWithPrivateKey Private => privateCert;
    }
}
