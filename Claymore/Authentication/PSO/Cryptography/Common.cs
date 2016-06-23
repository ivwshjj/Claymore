using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace Claymore.Authentication.PSO.Cryptography
{
    public class Common
    {
        public static byte[] Str2Byte(string Str)
        {
            return Convert.FromBase64String(Str);
        }
        public static string Byte2Str(Byte[] Byt)
        {
            return Convert.ToBase64String(Byt);
        }
        public static Byte[] NormalStr2Byte(string str)
        {
            return Encoding.Default.GetBytes(str);
        }
        public static string NormalByte2Str(byte[] Byt)
        {
            return Encoding.Default.GetString(Byt);
        }

       
    }
}
