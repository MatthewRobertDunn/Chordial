using Hive.Cryptography.Certificates;
using MS = System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto.Generators;

namespace Hive.Cryptography.Primitives
{
    public static class CertPrimitives
    {
        private static AsymmetricCipherKeyPair GenerateKeyPair(DerObjectIdentifier sec, string algorithm = "ECDSA")
        {
            //create keypair
            IAsymmetricCipherKeyPairGenerator bcKpGen = GeneratorUtilities.GetKeyPairGenerator(algorithm);
            bcKpGen.Init(new ECKeyGenerationParameters(sec, new SecureRandom()));
            return bcKpGen.GenerateKeyPair();
        }

        private static AsymmetricCipherKeyPair GenerateSigningKeypair()
        {
            var keyGen = new Ed448KeyPairGenerator();
            keyGen.Init(new Ed448KeyGenerationParameters(new SecureRandom()));
            return keyGen.GenerateKeyPair();
        }

        public static Pkcs12Store CreateStore(CertWithPrivateKey root, params CertWithPrivateKey[] certs)
        {
            var store = new Pkcs12Store();

            var issuerCertEntry = new X509CertificateEntry(root.Certificate);
            var rootAlias = root.Certificate.SubjectDN.ToString();
            store.SetCertificateEntry(rootAlias, issuerCertEntry);
            store.SetKeyEntry(rootAlias, new AsymmetricKeyEntry(root.PrivateKey), new[] { issuerCertEntry });


            foreach (var cert in certs)
            {
                var certEntry = new X509CertificateEntry(cert.Certificate);
                var alias = cert.Certificate.SubjectDN.ToString();
                store.SetCertificateEntry(alias, certEntry);
                store.SetKeyEntry(alias, new AsymmetricKeyEntry(cert.PrivateKey), new[] { certEntry, issuerCertEntry });
            }

            return store;
        }

        public static Pkcs12Store CreatePublicStore(X509Certificate root, params X509Certificate[] certs)
        {
            var store = new Pkcs12Store();

            var issuerCertEntry = new X509CertificateEntry(root);
            var rootAlias = root.SubjectDN.ToString();
            store.SetCertificateEntry(rootAlias, issuerCertEntry);

            foreach (var cert in certs)
            {
                var certEntry = new X509CertificateEntry(cert);
                var alias = cert.SubjectDN.ToString();
                store.SetCertificateEntry(alias, certEntry);
            }

            return store;
        }

        public static void Save(this Pkcs12Store store, string password = "password")
        {
            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), new SecureRandom());
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            File.WriteAllBytes(Path.Combine(folder, "hiveprivate.pfx"), stream.ToArray());
        }

        public static byte[] GetBytes(this Pkcs12Store store, string password = "password")
        {
            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), new SecureRandom());
            return stream.ToArray();
        }

        public static CertWithPrivateKey GetCertPrivate(this Pkcs12Store store, string alias)
        {
            var cert = store.GetCertificate(alias).Certificate;
            var priv = store.GetKey(alias).Key;
            return new CertWithPrivateKey(cert, priv);
        }

        public static Pkcs12Store Load(string password = "password")
        {
            Pkcs12Store store = new Pkcs12Store();
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            using (var stream = File.OpenRead(Path.Combine(folder, "hiveprivate.pfx")))
            {
                store.Load(stream, password.ToCharArray());
            }

            return store;
        }

        public static CertWithPrivateKey CreateRootCertificate(string subject)
        {
            var issuerKeys = GenerateKeyPair(SecObjectIdentifiers.SecP256r1);
            var cert = CreateRootCert(issuerKeys, issuerKeys.Private, subject);
            return new CertWithPrivateKey(cert, (ECPrivateKeyParameters)issuerKeys.Private);

        }

        public static MS.X509Certificate2 ToMicrosoftPrivate(this CertWithPrivateKey cert)
        {
            var store = CreateStore(cert);
            var stream = new MemoryStream();
            store.Save(stream, "password".ToCharArray(), new SecureRandom());

            return new MS.X509Certificate2(
                stream.ToArray(), "password",
                MS.X509KeyStorageFlags.PersistKeySet | MS.X509KeyStorageFlags.Exportable);
        }

        public static byte[] GetThumbprint(this X509Certificate cert)
        {
            var pub = (ECPublicKeyParameters)cert.GetPublicKey();
            return pub.Q.GetEncoded().SHAThree256();
        }

        public static CertWithPrivateKey CreateSigningCertificate(CertWithPrivateKey issuer, string subject)
        {
            var childKeys = GenerateSigningKeypair();
            var cert = CreateCert(childKeys, issuer.PrivateKey, subject, issuer.Certificate.SubjectDN);
            return new CertWithPrivateKey(cert, childKeys.Private);
        }

        public static CertWithPrivateKey CreateChildCertificate(CertWithPrivateKey issuer, string subject, string algorithm = "ECDSA")
        {
            var childKeys = GenerateKeyPair(SecObjectIdentifiers.SecP256r1, algorithm);

            var cert = CreateCert(childKeys, issuer.PrivateKey, subject, issuer.Certificate.SubjectDN);
            return new CertWithPrivateKey(cert, (ECPrivateKeyParameters)childKeys.Private);
        }

        private static X509Certificate CreateRootCert(AsymmetricCipherKeyPair childKeys, AsymmetricKeyParameter issuerPrivate, string subject)
        {
            return CreateCert(childKeys, issuerPrivate, subject, new X509Name(subject));
        }


        private static X509Certificate CreateCert(AsymmetricCipherKeyPair childKeys, AsymmetricKeyParameter issuerPrivate, string subject, X509Name issuerDN)
        {
            //create cert, sign with private key
            X509V3CertificateGenerator bcXgen = new X509V3CertificateGenerator();
            var subjectDN = new X509Name(subject);
            bcXgen.SetIssuerDN(issuerDN);
            bcXgen.SetSubjectDN(subjectDN);
            bcXgen.SetNotBefore(DateTime.Now);
            bcXgen.SetNotAfter(DateTime.Now.AddYears(100));
            bcXgen.SetSerialNumber(new BigInteger(256, new SecureRandom()));
            // Basic Constraints - certificate is allowed to be used as intermediate.
            bcXgen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

            bcXgen.SetPublicKey(childKeys.Public);
            ISignatureFactory bcSigFac = new Asn1SignatureFactory("SHA256WITHECDSA", issuerPrivate);
            var bcCert = bcXgen.Generate(bcSigFac);
            return bcCert;
        }
    }
}
