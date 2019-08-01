using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography
{
    public class AeadMessage
    {
        public byte[] InitializationVector { get; set; }

        public byte[] AdditionalData { get; set; }

        public byte[] CipherText { get; set; }
    }
}
