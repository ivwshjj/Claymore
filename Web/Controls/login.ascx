<%@ Control Language="C#" AutoEventWireup="true" Inherits="MyUserControlView<LoginModel>"  %>

<form  method="post">
    <table cellpadding="4" border="0px">
    
    <tr>
        <td>用户名</td>
        <td>
            <input name="account" type="text" maxlength="50" id="account" class="myTextbox" value="<%= Model.account %>"  />
        </td>
    </tr>
    <tr>
        <td>密码</td>
        <td>
            <input name="pwd" type="text" maxlength="50" id="txtPwd" class="myTextbox"  />
        </td>
    </tr>
    <tr>
        <td style="width: 80px"></td>
        <td>
            <input type="hidden" name="<%= ConfigManager.SSOKey %>" value="<%=HttpContext.Current.Request[ConfigManager.SSOKey] %>" />
            <input type="submit" value="提交" />
        </td>
    </tr>
</table>
</form>
