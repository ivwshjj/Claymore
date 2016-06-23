using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.SSO.Common
{
    /// <summary>
    /// 票据创建工具
    /// </summary>
    public class TicketCreator
    {
        public static void CreateSSO(string SiteID, string domain, string UserID, string UserName, string Data, string CreateDate, int minutes)
        {
            Ticket ticket = new SSOTicket(SiteID, UserID, UserName, Data, CreateDate);
            ticket.SaveTicket(domain, minutes);
        }

        public static void CreateSSO(string SiteID, string UserID, string UserName, string Data, string CreateDate, int minutes)
        {
            Ticket ticket = new SSOTicket(SiteID, UserID, UserName, Data, CreateDate);
            ticket.SaveTicket(ConfigManager.SiteID, minutes);
        }
    }
}
