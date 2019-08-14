using Hive.Cryptography.Certificates;
using Hive.Cryptography.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var signature = store.Channel.PrivateKey.Sign("Hello");
            var isValid = store.Channel.PublicKey.VerifySign("Hello", signature);
            Assert.IsTrue(isValid);

            var isInvalid = store.Channel.PublicKey.VerifySign("Stuiff", signature);
            Assert.IsFalse(isInvalid);
        }
    }
}
