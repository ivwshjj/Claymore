<%@ Page Language="C#" %>
<%@ Import Namespace="Claymore.PSO.Common" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <script runat="server">
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                //已经登录，则跳转到默认页面
                this.Response.Redirect("/manage/default.aspx");
            }
            //目标地址
            string url = HttpContext.Current.Request["ReturnUrl"];

            string ssoresponse = HttpContext.Current.Request.QueryString[Claymore.Authentication.SSO.Common.ConfigManager.SSOKey];

            if (string.IsNullOrEmpty(ssoresponse) == false)
            {
                //收到了来自主站点的响应
                SSOResponse sr = new SSOResponse(ssoresponse);
                //创建本地的ticket
                PSOTicket tc = sr.CreatePSOTicket() as PSOTicket;
                //保存ticket 到cookie中
                tc.SaveTicket(ConfigManager.SiteID, 10);
                //tc.Createdate 这个时间是ticket 在主站点的创建时间，需要时请进行判断
                //标记登录,这里使用的是form登录
                Claymore.Authentication.MyFormsPrincipal<PSOTicket>.SignIn(tc.Username, tc, 10);
                //页面跳转
                if (string.IsNullOrEmpty(sr.TargetUrl))
                {
                    this.Response.Redirect(url);
                }
                this.Response.Redirect(sr.TargetUrl);
            }
            else
            {
                //没有收到来自主站点的请求参数
                Ticket tc = new PSOTicket();
                //加载本地的票据信息
                if (!tc.LoadTicket(ConfigManager.SiteID))
                {
                    //没有本地登录票据信息
                    //创建子站点的PSO请求
                    PSORequest request = new PSORequest();
                    //设置返回的URL
                    request.TargetUrl = url;
                    string requeststr = request.CreateHash();

                    //转到认证页面,如果已经认证就返回,没有认证就要登录
                    request.CreateSSOUrl();
                    //跳转到主站点进行验证
                    this.Response.Redirect(request.CreateSSOUrl());
                }
                else
                {
                    //加载成功,存在本地票据，显示页面
                }
            }
        }
    </script>
</head>
<body ng-app="myApp" class="ng-app:myapp" id="ng-app">
    
    <div id="divdata">

    </div>
    <a href="/pso/logout.aspx">退出</a>
</body>
</html>
