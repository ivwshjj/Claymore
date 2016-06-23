using Claymore.Authentication.PSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.PSO.Common
{
    /// <summary>
    /// 主站点返回响应类
    /// </summary>
    public class SSOResponse
    {
        private string Key = ConfigManager.Key;

        private string IV = ConfigManager.IV;

        public string TargetUrl { get; private set; }

        private string Response;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="SSOResponse">从主站点接收到的返回数据,数据已经经过Base64Url</param>
        public SSOResponse(string SSOResponse)
        {
            Response = Ticket.Base64UrlToData(SSOResponse);
            Decrypter dc = new Decrypter(Key, IV);
            string data = dc.DecryptString(Response); //dc.DecryptString(Response.Trim().Replace(" ", "+"));
            string[] ls = data.Split('|');
            TargetUrl = ls[5];
        }

        /// <summary>
        /// 创建一个PSO的本地票据
        /// </summary>
        /// <returns></returns>
        public PSOTicket CreatePSOTicket()
        {
            Decrypter dc = new Decrypter(Key, IV);
            string data = dc.DecryptString(Response); //dc.DecryptString(Response.Trim().Replace(" ", "+"));
            string[] ls = data.Split('|');
            PSOTicket tc = new PSOTicket(ls[1], ls[2], ls[3], ls[4], DateTime.Now.AddDays(1));
            TargetUrl = ls[5];
            return tc;
        }
    }
}
