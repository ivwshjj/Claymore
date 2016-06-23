using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Claymore.Authentication.SSO.Cryptography
{
    /// <summary>
    /// 密钥管理类
    /// </summary>
    public class KeyMaker
    {
        private string key;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        private string iv;

        public string Iv
        {
            get { return iv; }
            set { iv = value; }
        }


        public KeyMaker()
        {
            System.Security.Cryptography.TripleDES des = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.GenerateKey();
            des.GenerateIV();
            key = Common.Byte2Str(des.Key);
            iv = Common.Byte2Str(des.IV);
        }
    }
}
