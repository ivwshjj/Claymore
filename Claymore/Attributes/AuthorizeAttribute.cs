using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeAttribute : Attribute
    {
        private string _user;
        private List<string> _users = new List<string>();
        private string _role;
        private List<string> _roles = new List<string>();


        public string Users
        {
            get { return _user; }
            set
            {
                _user = value;
                _users = new List<string>( value.Split(','));
            }
        }

        public string Roles
        {
            get { return _role; }
            set
            {
                _role = value;
                _roles = new List<string>( value.Split(','));
            }
        }


        public virtual bool AuthenticateRequest(HttpContext context)
        {
            if (context.Request.IsAuthenticated == false)
                return false;

            if (_users != null &&
                _users.Contains(context.User.Identity.Name) == false)
                return false;

            if (_roles != null && IsInRole(context) == false)
                return false;

            return true;
        }

        protected bool IsInRole(HttpContext context)
        {
            bool flag = false;
            foreach (string s in _roles)
            {
                flag = context.User.IsInRole(s);
                if (flag)
                {
                    break;
                }
            }
            return flag;
        }

        public virtual object RequestResult(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("请求验证为不通过时的返回信息");
            context.Response.End();
            return null;
        }
    }
}
