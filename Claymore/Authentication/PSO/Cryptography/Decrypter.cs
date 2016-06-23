using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Claymore.Authentication.PSO.Cryptography
{
    /// <summary>
    /// 解密类
    /// </summary>
    public class Decrypter
    {
        private ICryptoTransform decrypter;

        public Decrypter(string Key, string IV)
        {
            System.Security.Cryptography.TripleDES des = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create();
            des.Key = Common.Str2Byte(Key);
            des.IV = Common.Str2Byte(IV);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            decrypter = des.CreateDecryptor();
        }

        public string DecryptString(string InputString)
        {
            byte[] input = Common.Str2Byte(InputString);
            byte[] output = decrypter.TransformFinalBlock(input, 0, input.Length);
            return Common.NormalByte2Str(output);
        }
    }
}
