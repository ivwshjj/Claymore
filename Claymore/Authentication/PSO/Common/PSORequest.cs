using Claymore.Authentication.PSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore.Authentication.PSO.Common
{
    /// <summary>
    /// PSO 请求类
    /// </summary>
    public class PSORequest
    {
        //配制文件中的密钥
        private string Key = ConfigManager.Key;

        //配制文件中的密匙
        private string IV = ConfigManager.IV;

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
        /// 子站点 SSO请求创建
        /// </summary>
        public PSORequest()
        {
            siteid = ConfigManager.SiteID;
            createdate = DateTime.Now.ToString();
            returnurl = System.Web.HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        /// 子站点 SSO请求创建
        /// </summary>
        /// <param name="SiteId">站点ID</param>
        /// <param name="Key">加密密钥</param>
        /// <param name="IV">初始化向量</param>
        public PSORequest(string SiteId, string Key, string IV)
        {
            siteid = SiteId;
            this.Key = Key;
            this.IV = IV;
            createdate = DateTime.Now.ToString();
            returnurl = System.Web.HttpContext.Current.Request.Url.ToString();
        }

        public string CreateHash()
        {
            Encrypter en = new Encrypter(Key, IV);
            return siteid + "$" + en.EncryptString(createdate + "|" + Returnurl + "|" + TargetUrl);
        }

        /// <summary>
        /// 创建SSO的请求URL
        /// </summary>
        /// <returns></returns>
        public string CreateSSOUrl()
        {
            bool flag = ConfigManager.KeeperUrl.IndexOf("?") > 0;
            return ConfigManager.KeeperUrl + (flag ? "&" : "?") + ConfigManager.SSOKey + "=" + Ticket.Base64StringToUrl(CreateHash()); //HttpContext.Current.Server.UrlEncode(CreateHash());
        }

        public string D(string input)
        {
            Decrypter d=new Decrypter(Key,IV);
            return d.DecryptString(input);
        }
    }
}
