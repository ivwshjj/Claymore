using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Authentication.PSO.Common
{
    /// <summary>
    /// 票据创建工具
    /// </summary>
    public class TicketCreator
    {
        public static void CreatePSO(string domain, string UserID, string UserName, string Data, string CreateDate,int minutes)
        {
            Ticket ticket = new PSOTicket(UserID, UserName, Data, CreateDate,DateTime.Now.AddMinutes(minutes));
            ticket.SaveTicket(domain,minutes);
        }

        public static void CreatePSO(string UserID, string UserName, string Data, string CreateDate,int minutes)
        {
            Ticket ticket = new PSOTicket(UserID, UserName, Data, CreateDate, DateTime.Now.AddMinutes(minutes));
            ticket.SaveTicket(System.Configuration.ConfigurationManager.AppSettings["domain"],minutes);
        }

        
    }
}
