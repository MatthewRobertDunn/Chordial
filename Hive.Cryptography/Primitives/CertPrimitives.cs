using Hive.Cryptography.Certificates;
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
using System.Text;

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

        public static Pkcs12Store CreateStore(CertWithPrivateKey root,  IEnumerable<CertWithPrivateKey> certs)
        {
            var store = new Pkcs12Store();

            foreach (var cert in certs)
            {
                var issuerCertEntry = new X509CertificateEntry(cert.Certificate);
                var alias = cert.Certificate.SubjectDN.ToString();
                store.SetCertificateEntry(alias, issuerCertEntry);
                store.SetKeyEntry(alias, new AsymmetricKeyEntry(cert.PrivateKey),);
            }

        }


        public static CertWithPrivateKey CreateRootCertificate()
        {
            var issuerKeys = GenerateKeyPair(SecObjectIdentifiers.SecP256r1);
            var cert = CreateRootCert(issuerKeys, issuerKeys.Private, "CN=HiveRoot, O=Bitcoin");
            return new CertWithPrivateKey(cert, (ECPrivateKeyParameters)issuerKeys.Private);

        }

        public static byte[] GetThumbprint(this X509Certificate cert)
        {
            var pub = (ECPublicKeyParameters)cert.GetPublicKey();
            return pub.Q.GetEncoded().SHAThree256();
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
