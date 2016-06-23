using Claymore.Authentication.SSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// 票据类,保存已经加密的文本
    /// </summary>
    public abstract class Ticket
    {
        //加密类
        [NonSerialized]
        protected Encrypter enc;
        //解密类
        [NonSerialized]
        protected Decrypter dec;
        protected string Key = ConfigManager.Key;
        protected string IV = ConfigManager.IV;

        protected string userid = "";



        //用户ID
        public string Userid
        {
            get { return dec.DecryptString(userid); }
            set { userid = enc.EncryptString(value); }
        }
        protected string username = "";

        //用户名
        public string Username
        {
            get { return dec.DecryptString(username); }
            set { username = enc.EncryptString(value); }
        }
        protected string ticketdata = "";

        //票据信息
        public string Ticketdata
        {
            get { return dec.DecryptString(ticketdata); }
            set { ticketdata = enc.EncryptString(value); }
        }

        protected string createdate = "";

        //创建时间
        public string Createdate
        {
            get { return dec.DecryptString(createdate); }
            set { createdate = enc.EncryptString(value); }
        }

        protected string expires = string.Empty;

        public string Expires
        {
            get { return dec.DecryptString(expires); }
            set { expires = enc.EncryptString(value); }
        }


        //根据域名加载票据,当然也可以不是域名
        public bool LoadTicket(string domain)
        {
            System.Web.HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[domain];
            if (cookie == null ||
                string.IsNullOrEmpty(cookie["Expires"]))
            {
                return false;
            }
            DateTime dtExpires = DateTime.Now;
            string cookieexpires = dec.DecryptString(cookie["Expires"]);
            if (DateTime.TryParse(cookieexpires, out dtExpires) && dtExpires > DateTime.Now)
            {
                userid = cookie["UserID"];
                username = cookie["UserName"];
                ticketdata = cookie["TicketDate"];
                Expires = cookie["Expires"];
                return true;
            }
            return false;
        }

        //保存票据到cookie中,过期时间为一天后
        public void SaveTicket(string domain,int minutes)
        {
            System.Web.HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[domain];
            if (cookie == null)
            {
                cookie = new System.Web.HttpCookie(domain);
            }
            cookie.Name = domain;
            if (Regex.IsMatch(domain, @"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$"))
            {
                cookie.Domain = domain;
            }
            cookie["UserID"] = userid;
            cookie["UserName"] = username;
            cookie["TicketDate"] = ticketdata;
            cookie["Expires"] = enc.EncryptString(DateTime.Now.AddDays(1).ToString());
            cookie.Expires = DateTime.Now.AddMinutes(minutes);
            System.Web.HttpContext.Current.Response.SetCookie(cookie);
        }

        public void SingOut(string domain)
        {
            System.Web.HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[domain];
            if (cookie == null)
            {
                return;
            }
            cookie["UserID"] = string.Empty;
            cookie["UserName"] = string.Empty;
            cookie["TicketDate"] = string.Empty;

            cookie["Expires"] = string.Empty;
            cookie.Expires = DateTime.Now.AddDays(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            //System.Web.HttpContext.Current.Response.Cookies.Remove(domain);
        }

        /// <summary>
        /// 从字符转换为适用于URL的Base64编码字符串
        /// + 转成 *
        /// / 转成 -
        /// = 转成 .
        /// < /summary>
        public static string Base64StringToUrl(string data)
        {
            byte[] token = System.Text.Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(token).Replace('+', '*')
                .Replace('/', '-')
                .Replace('=', '.');
        }

        /// <summary>
        /// 从适用于URL的Base64编码字符串转换为原来的数据
        /// * 还原成 +
        /// - 还原成 /
        /// . 还原成 =
        /// < /summary>
        public static string Base64UrlToData(string data)
        {
            byte[] token = Convert.FromBase64String(data);
            string result = System.Text.Encoding.UTF8.GetString(token);
            return result.Replace('*', '+')
                .Replace('-', '/')
                .Replace('.', '=');
        }
    }
}
