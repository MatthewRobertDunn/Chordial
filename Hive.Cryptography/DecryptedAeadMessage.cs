using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Cryptography
{
    public class DecryptedAeadMessage
    {
        public string AdditionalData { get; set; }
        public string ProtectedData { get; set; }
    }
}
