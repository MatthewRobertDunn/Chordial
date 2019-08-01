using System;
using System.Collections.Generic;
using System.Text;
namespace Hive.Cryptography.Primitives
{
    public static class TextPrimitives
    {
        public static string FromUTF8(this byte[] base64Text)
        {
            if (base64Text == null)
                return null;
            return UTF8Encoding.Default.GetString(base64Text);
        }
    }
}
