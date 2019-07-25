using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using MS = System.Security.Cryptography;
using System.Text;
using BC = Org.BouncyCastle;
using Org.BouncyCastle.Asn1.X509;
using System.IO;

namespace Hive.Overlay.Peer.Crypto
{
    public class KeyGen
    {
        public MS.X509Certificates.X509Certificate2 Generate()
        {
            //create keypair
            IAsymmetricCipherKeyPairGenerator bcKpGen = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            bcKpGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom()));
            AsymmetricCipherKeyPair bcSubjKeys = bcKpGen.GenerateKeyPair();

            IAsymmetricCipherKeyPairGenerator issuerGen = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            issuerGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom()));
            AsymmetricCipherKeyPair issuerKeys = issuerGen.GenerateKeyPair();

            X509Certificate bcCert = CreateCert(bcSubjKeys, issuerKeys, "CN=Hive", "CN=HiveRoot");
            X509Certificate issuerCert = CreateCert(issuerKeys, issuerKeys, "CN=HiveRoot, O=Bitcoin", "CN=HiveRoot");


            //export keypair to a microsoft format in a roundabout way
            var store = new Pkcs12Store();
            string friendlyName = bcCert.SubjectDN.ToString();

            var certificateEntry = new X509CertificateEntry(bcCert);
            var issuerCertEntry = new X509CertificateEntry(issuerCert);
            store.SetCertificateEntry("issuer", issuerCertEntry);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(bcSubjKeys.Private), new[] { certificateEntry });

            const string password = "password";
            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), new SecureRandom());

            File.WriteAllBytes(@"c:\temp\foo.pfx", stream.ToArray());


            var bitcoinCert = new MS.X509Certificates.X509Certificate2(issuerCert.GetEncoded(), string.Empty);

            //conver to MS format.
            var transportCert =
                new MS.X509Certificates.X509Certificate2(
                    stream.ToArray(), password,
                    MS.X509Certificates.X509KeyStorageFlags.PersistKeySet | MS.X509Certificates.X509KeyStorageFlags.Exportable);


            var ch = new MS.X509Certificates.X509Chain();

            ch.ChainPolicy.RevocationMode = MS.X509Certificates.X509RevocationMode.NoCheck;
            ch.ChainPolicy.RevocationFlag = MS.X509Certificates.X509RevocationFlag.ExcludeRoot;
            ch.ChainPolicy.VerificationFlags = MS.X509Certificates.X509VerificationFlags.AllowUnknownCertificateAuthority;

            ch.ChainPolicy.ExtraStore.Add(bitcoinCert);
            var isValid = ch.Build(transportCert);

            return transportCert;
        }

        private static X509Certificate CreateCert(AsymmetricCipherKeyPair bcSubjKeys, AsymmetricCipherKeyPair issuerKeys, string subject, string issuer)
        {
            //create cert, sign with private key
            X509V3CertificateGenerator bcXgen = new X509V3CertificateGenerator();
            var subjectDN = new X509Name(subject);
            var issuerDN = new X509Name(issuer);
            bcXgen.SetIssuerDN(issuerDN);
            bcXgen.SetSubjectDN(subjectDN);
            bcXgen.SetNotBefore(DateTime.Now);
            bcXgen.SetNotAfter(DateTime.Now.AddYears(100));
            bcXgen.SetPublicKey(bcSubjKeys.Public);
            bcXgen.SetSerialNumber(new BC.Math.BigInteger(256, new SecureRandom()));

            ISignatureFactory bcSigFac = new Asn1SignatureFactory("SHA256WITHECDSA", issuerKeys.Private);
            var bcCert = bcXgen.Generate(bcSigFac);
            return bcCert;
        }
    }
}
