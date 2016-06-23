using Claymore;
using Claymore.Authentication;
using Claymore.Authentication.SSO.Common;
using System;
using System.Collections.Generic;
using System.Web;
using Web.Code;

namespace WebPSO
{
    public class PageController
    {
        [Claymore.Action]
        [Claymore.PageUrl(Url="/d.aspx")]
        public object Page()
        {
            return new Claymore.UcResult("/Controls/login.ascx", null);
        }

        [Claymore.Action]
        [Claymore.PageUrl(Url = "/keeper.aspx")]
        public object Keeper(string SSOToken)
        {
            string RSKey = ConfigManager.SSOKey;
            string loginurl = "/sso/login.aspx";
            if (string.IsNullOrEmpty( SSOToken)==false)
            {
                //收到来自子站点的请求参数
                //PSORequest pr = new PSORequest(psorequest, "qP70966AcZCQyXR+3P1mfjmqqxdkagom", "FnZ+19kJbQ8=");
                PSORequest pr = new PSORequest(SSOToken);
                SSOResponse sr = new SSOResponse(pr);
                Ticket tc = new SSOTicket();
                //验证是否已经登录且存在SSO票据
                if (HttpContext.Current.Request.IsAuthenticated && tc.LoadTicket(ConfigManager.SiteID))
                {
                    //用户已经在主站点登录，创建SSO的返回请求
                    string ssorequest = sr.CreateResponseString(tc);
                    bool flag = pr.Returnurl.IndexOf("?") > 0;
                    string url = pr.Returnurl + (flag ? "&" : "?") + ConfigManager.SSOKey + "=" + ssorequest;
                    //返回到跳转过来的成员站点,并加上SSO票据
                    //Context.Response.Redirect(url);
                    return new RedirectResult(url);
                }
                else
                {
                    //SSO票据加载失败,证明还没有进行登录,转到登录页面
                    //Context.Response.Redirect(loginurl + "?" + RSKey + "=" + psorequest);
                    return new RedirectResult(loginurl + "?" + RSKey + "=" + SSOToken);
                }
            }
            else
            {
                //没有收到客户端的PSO票据,直接要求登录
                return new RedirectResult(loginurl + "?" + RSKey + "=" + SSOToken);
            }
        }

        [Claymore.Action]
        [Claymore.PageUrl(Url = "/sso/login.aspx")]
        public object Login(string SSOToken,LoginModel model)
        {
            if (model !=null && model.account == "admin" && model.pwd == "admin")
            {
                SSOTicket ticket = new SSOTicket();
                ticket.Createdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ticket.Expires = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                ticket.Ticketdata = Claymore.Helper.MyJsonHelper.SerializeObject(model);
                ticket.Userid = "1";
                ticket.Username = model.account;
                //创建本地SSO Ticket,保存到cookie
                ticket.SaveTicket(ConfigManager.SiteID, 60);
                //修改登录状态
                MyFormsPrincipal<SSOTicket>.SignIn(model.account, ticket, 10);
                string psorequest = SSOToken;
                if (psorequest != null)
                {
                    //收到来自子站点的请求
                    PSORequest pr = new PSORequest(psorequest);
                    //创建响应的SSO请求返回给子站点
                    SSOResponse sr = new SSOResponse(pr);
                    string and = pr.Returnurl.IndexOf("?") > 0 ? "&" : "?";
                    string url = pr.Returnurl + and + ConfigManager.SSOKey + "=" + sr.CreateResponseString(ticket);
                    //跳转回子站点
                    return new RedirectResult(url);
                }
                else
                {
                    //没有收到，登录成功后跳转到默认页面
                    return new RedirectResult("/manage/index.aspx");
                }
            }
            return new UcResult("/Controls/login.ascx", model);
        }
    }
}