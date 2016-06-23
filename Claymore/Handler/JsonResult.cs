using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Claymore.Extensions;
using System.Reflection;
using Claymore.Helper;

namespace Claymore
{
    /// <summary>
    /// 一个Json对象结果
    /// </summary>
    public sealed class JsonResult : IActionResult
    {
        public object Model { get; private set; }

        public JsonResult(object model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            this.Model = model;
        }

        void IActionResult.Ouput(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            JsonHelper json = new JsonHelper();
            context.Response.Write(json);
        }
    }
}
