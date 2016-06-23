using System;
using Claymore.Authentication.SSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// PSO 请求类
    /// </summary>
    public class PSORequest
    {
        private string siteid;

        /// <summary>
        /// 子站点ID
        /// </summary>
        public string Siteid
        {
            get { return siteid; }
            set { siteid = value; }
        }
        private string createdate;

        /// <summary>
        /// 创建时间
        /// </summary>
        public string Createdate
        {
            get { return createdate; }
            set { createdate = value; }
        }
        private string returnurl;

        /// <summary>
        /// 返回地址
        /// </summary>
        public string Returnurl
        {
            get { return returnurl; }
            set { returnurl = value; }
        }

        private string _targeturl;

        /// <summary>
        /// 目标地址
        /// </summary>
        public string TargetUrl
        {
            get { return _targeturl; }
            set { _targeturl = value; }
        }

        /// <summary>
        /// 主站点使用,反解子站的参数
        /// </summary>
        /// <param name="HashString">子站点的请求参数【子站点ID｜创建时间|返回地址|目标地址】Base64URL字符串</param>
        /// <param name="Key">解密密钥</param>
        /// <param name="IV">初始化向量</param>
        public PSORequest(string HashString, string Key, string IV)
        {
            string hash = Ticket.Base64UrlToData(HashString);
            string[] rls = hash.Split('$');
            Decrypter de = new Decrypter(Key, IV);
            string rs = de.DecryptString(rls[1]);
            string[] sp = rs.Split('|');
            siteid = rls[0];
            createdate = sp[0];
            returnurl = sp[1];
            _targeturl = sp[2];
        }

        /// <summary>
        /// 主站点使用,反解子站的参数
        /// </summary>
        /// <param name="HashString">子站点的请求参数【子站点ID｜创建时间|返回地址|目标地址】Base64URL字符串</param>
        /// <param name="Key">解密密钥</param>
        /// <param name="IV">初始化向量</param>
        public PSORequest(string HashString)
        {
            string hash = Ticket.Base64UrlToData(HashString);
            string[] rls = hash.Split('$');
            string Key,IV;
            KeyManager.GetKeyBySiteID(rls[0], out Key, out IV);
            Decrypter de = new Decrypter(Key, IV);
            string rs = de.DecryptString(rls[1]);
            string[] sp = rs.Split('|');
            siteid = rls[0];
            createdate = sp[0];
            returnurl = sp[1];
            _targeturl = sp[2];
        }
    }
}
