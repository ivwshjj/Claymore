<%@ Control Language="C#" AutoEventWireup="true" Inherits="MyUserControlView<UserModel>" %>

<table cellpadding="4" border="0px">
    <tr>
        <td style="width: 80px">客户ID</td>
        <td>
            <input name="CustomerName" type="text" maxlength="50" id="txtCustomerName" class="myTextbox" value="<%= Model.userId %>" />
        </td>
    </tr>
    <tr>
        <td>客户名称</td>
        <td>
            <input name="ContactName" type="text" maxlength="50" id="txtContactName" class="myTextbox" value="<%=Model.userName %>" />
        </td>
    </tr>
    <tr>
        <td>密码</td>
        <td>
            <input name="ContactName" type="text" maxlength="50" id="txtPwd" class="myTextbox" value="<%=Model.passWord %>" />
        </td>
    </tr>
    <tr>
        <td>角色</td>
        <td>
            <input name="ContactName" type="text" maxlength="50" id="txtRoleId" class="myTextbox" value="<%=Model.roleId %>" />
        </td>
    </tr>
</table>
