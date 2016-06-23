<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script runat="server">
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            PSORequest request = new PSORequest();
            string data = request.CreateSSOUrl();
            this.Response.Write(data);
            string d= Claymore.Authentication.PSO.Common.Ticket.Base64UrlToData("UFNPU2l0ZSRkdjQydzQzcjc1dnl1dEdaaGltVFcvdHZ2QTVZV0NaQ3c2Z2FKUkVDa3Z0TXFhaVd1czdiVENRc0JsSS8wL09uOEhMV1UyYzIyNHM9");
            this.Response.Write("<br />" + d);
            this.Response.Write("<br />" + request.D("dv42w43r75vyutGZhimTW/tvvA5YWCZCw6gaJRECkvtMqaiWus7bTCQsBlI/0/On8HLWU2c224s="));
        }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
