using Claymore.Authentication.SSO.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// PSOTicket类,即是在子站点的Ticket
    /// </summary>
    [Serializable]
    public class PSOTicket : Ticket
    {
        public PSOTicket(string UserID, string UserName, string TicketData, string CreateDate)
        {
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
            userid = enc.EncryptString(UserID);
            username = enc.EncryptString(UserName);
            ticketdata = enc.EncryptString(TicketData);
            createdate = enc.EncryptString(CreateDate);
        }
        
        public PSOTicket(string Key, string IV)
        {
            this.Key = Key;
            this.IV = IV;
            enc = new Encrypter(Key, IV);
            dec = new Decrypter(Key, IV);
        }
    }
}
