using Hive.Cryptography.Certificates;
using Hive.Cryptography.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Tests
{
    [TestClass]
    public class CryptoTests
    {
        [TestMethod]
        public void TestSignature()
        {
            var store = new CertificateStore();
            store.Generate();
            var privateKey = (Ed448PrivateKeyParameters)store.Channel.PrivateKey;
            var publicKey = (Ed448PublicKeyParameters)store.Channel.PublicKey;


            var signature = privateKey.Sign("Hello");
            var isValid = publicKey.VerifySign("Hello", signature);
            Assert.IsTrue(isValid);

            var isInvalid = publicKey.VerifySign("Stuiff", signature);
            Assert.IsFalse(isInvalid);
        }
    }
}
