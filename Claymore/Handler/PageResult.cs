using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    public class PageResult:IActionResult
    {
        public string VirtualPath { get; private set; }
        public object Model { get; private set; }

        public PageResult(string virtualPath)
            : this(virtualPath, null)
        {
        }

        public PageResult(string virtualPath, object model)
        {
            this.VirtualPath = virtualPath;
            this.Model = model;
        }

        #region IActionResult 成员

        public void Ouput(System.Web.HttpContext context)
        {
            if (string.IsNullOrEmpty(this.VirtualPath))
                this.VirtualPath = context.Request.FilePath;

            context.Response.ContentType = "text/html";
            string html = PageExecutor.Render(context, VirtualPath, Model);
            context.Response.Write(html);
        }

        #endregion
    }
}
