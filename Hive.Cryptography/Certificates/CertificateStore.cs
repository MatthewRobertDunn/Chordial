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

        private const string rootCertDn = "CN=HiveRoot,O=Bitcoin";
        private const string transportCertDn = "CN=Transport";
        private const string channelCertDn = "CN=Channel";
        private const string privateCertDn = "CN=Private";

        /// <summary>
        /// Creates a brand new set of hive keys for use.
        /// </summary>
        public void Generate()
        {
            rootCert = CertPrimitives.CreateRootCertificate(rootCertDn);
            HiveAddress = rootCert.Certificate.GetThumbprint();
            Transport = CertPrimitives.CreateChildCertificate(rootCert, transportCertDn, "ECDSA");
            Channel = CertPrimitives.CreateSigningCertificate(rootCert, channelCertDn);
            Private = CertPrimitives.CreateChildCertificate(rootCert, privateCertDn, "ECDH");
            IsLoaded = true;
        }


        public void Save()
        {
            var store = CertPrimitives.CreateStore(rootCert, Transport, Channel, Private);
            store.Save();
        }

        public void Load()
        {
            var store = CertPrimitives.Load();
            this.rootCert = store.GetCertPrivate(rootCertDn);
            HiveAddress = rootCert.Certificate.GetThumbprint();

            Transport = store.GetCertPrivate(transportCertDn);
            Channel = store.GetCertPrivate(channelCertDn);
            Private = store.GetCertPrivate(privateCertDn);

            IsLoaded = true;
        }


        public byte[] ToPublicPfxBytes()
        {
            var store = CertPrimitives.CreatePublicStore(rootCert.Certificate, Transport.Certificate, Channel.Certificate, Private.Certificate);
            return store.GetBytes();
        }

        public bool IsLoaded { get; private set; }

        public byte[] HiveAddress { get; private set; }

        public CertWithPrivateKey Transport { get; private set; }

        public CertWithPrivateKey Channel { get; private set; }

        public CertWithPrivateKey Private { get; private set; }
    }
}
