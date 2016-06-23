<%@ Page Language="C#" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <script src="/js/jquery.js"></script>
</head>
<body ng-app="myApp" class="ng-app:myapp" id="ng-app">
    <header ng-controller="navController">
        <nav-info info="navs"></nav-info>
    </header>
    <%--可以在 web.config 设置禁止非管理员用户访问本页。--%>

    <h3>这是一个【管理员】用户才能查看的页面</h3>

    IsAuthenticated: <%= Request.IsAuthenticated %>
    <br />
    用户名: <% PSOTicket t = new PSOTicket();%>
    <% if (t.LoadTicket(ConfigManager.SiteID))
       {%>
    <%= t.Username %>
    <%  
            }
    %>
    
    <div id="divdata">

    </div>
    <a href="/pso/logout.aspx">退出</a>
    <script>
        $(document).ready(function () {
            $.ajax({
                url: "/AjaxLoginAction/LoginName.do",
                type: 'post',
                success: function (data) {
                    $("#divdata").html("用户:" + data);
                }
            });
        });
    </script>

</body>
</html>
