﻿using System;
using System.Collections.Generic;
using System.Text;
namespace Hive.Cryptography.Primitives
{
    public static class TextPrimitives
    {
        public static string FromUTF8(this byte[] data)
        {
            if (data == null)
                return null;
            return UTF8Encoding.Default.GetString(data);
        }

        public static byte[] ToUTF8Bytes(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return new byte[0];

            return ASCIIEncoding.UTF8.GetBytes(data);
        }

        public static string ToBase64(this byte[] source)
        {
            return Convert.ToBase64String(source)
                .TrimEnd(new char[] { '=' }).Replace('+', '-').Replace('/', '_');
        }

        public static byte[] FromBase64(this string data)
        {
            string incoming = data.Replace('_', '/').Replace('-', '+');
            switch (data.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            return Convert.FromBase64String(incoming);
        }
    }
}
