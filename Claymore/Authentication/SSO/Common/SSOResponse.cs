using Claymore.Authentication.SSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    public class SSOResponse
    {
        private string Key = ConfigManager.Key;

        private string IV = ConfigManager.IV;

        public string TargetUrl { get; private set; }
        private PSORequest request;


        public SSOResponse(PSORequest Request,string Key,string IV)
        {
            request = Request;
            this.IV = IV;
            this.Key = Key;
            TargetUrl = request.TargetUrl;
        }

        public SSOResponse(PSORequest Request)
        {
            request = Request;
            KeyManager.GetKeyBySiteID(request.Siteid, out this.Key, out this.IV);
            TargetUrl = request.TargetUrl;
        }

        /// <summary>
        /// 将票据生成到URL参数中,调用Base64StringToUrl方法
        /// </summary>
        /// <param name="ticket">票据信息</param>
        /// <param name="Key">解密密钥</param>
        /// <param name="IV">初始化向量</param>
        /// <returns></returns>
        public string CreateResponseString(Ticket ticket)
        {
            ticket.Createdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string data = request.Siteid + "|" + ticket.Userid + "|" + ticket.Username + "|" + ticket.Ticketdata + "|" + ticket.Createdate + "|" + TargetUrl;
            Encrypter enc = new Encrypter(Key, IV);
            string temp= enc.EncryptString(data);
            return Ticket.Base64StringToUrl(temp);
        }
    }
}
