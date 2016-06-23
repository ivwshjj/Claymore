using Claymore.Authentication.SSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// 票据类,保存已经加密的文本
    /// </summary>
    [Serializable]
    public class SSOTicket : Ticket
    {
        public SSOTicket(string SiteID, string UserID, string UserName, string TicketData, string CreateDate)
        {
            KeyManager.GetKeyBySiteID(SiteID, out Key, out IV);
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
            userid = enc.EncryptString(UserID);
            username = enc.EncryptString(UserName);
            ticketdata = enc.EncryptString(TicketData);
            createdate = enc.EncryptString(CreateDate);
        }
        /// <summary>
        /// 根据站点ID读取本地PSOSite文件夹中的Key与IV
        /// </summary>
        /// <param name="SiteID"></param>
        public SSOTicket(string SiteID)
        {
            KeyManager.GetKeyBySiteID(SiteID, out Key, out IV);
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
        }

        public SSOTicket(string Key,string IV)
        {
            this.Key = Key;
            this.IV = IV;
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
        }

        public SSOTicket()
        {  
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
        }
    }
}
