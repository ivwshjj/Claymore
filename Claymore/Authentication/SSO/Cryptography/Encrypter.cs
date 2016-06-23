using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Claymore.Authentication.SSO.Cryptography
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class Encrypter
    {
        private ICryptoTransform encrypter;

        public Encrypter(string Key, string IV)
        {
            System.Security.Cryptography.TripleDES des = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create();
            des.Key = Common.Str2Byte(Key);
            des.IV = Common.Str2Byte(IV);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            encrypter = des.CreateEncryptor();
        }

        public string EncryptString(string InputString)
        {
            byte[] input = Common.NormalStr2Byte(InputString);
            byte[] output = encrypter.TransformFinalBlock(input, 0, input.Length);
            return Common.Byte2Str(output);
        }
    }
}
