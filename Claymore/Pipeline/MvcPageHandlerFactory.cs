using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Claymore
{
    internal sealed class AspnetPageHandlerFactory : PageHandlerFactory { }

    public sealed class MvcPageHandlerFactory : IHttpHandlerFactory
    {   
        public static IHttpHandler TryGetHandler(HttpContext context)
        {
            string vPath = UrlHelper.GetRealVirtualPath(context, context.Request.FilePath);

            InvokeInfo vkInfo = ReflectionHelper.GetActionInvokeInfo(vPath);
            if (vkInfo == null)
                return null;

            return ActionHandler.CreateHandler(vkInfo);
        }

        private AspnetPageHandlerFactory _msPageHandlerFactory;

        IHttpHandler IHttpHandlerFactory.GetHandler(HttpContext context,
                            string requestType, string virtualPath, string physicalPath)
        { 
            string requestPath = context.Request.Path;
            string vPath = UrlHelper.GetRealVirtualPath(context, requestPath);

            InvokeInfo vkInfo = ReflectionHelper.GetActionInvokeInfo(vPath);

            if (vkInfo == null && requestPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                if (_msPageHandlerFactory == null)
                    _msPageHandlerFactory = new AspnetPageHandlerFactory();

                return _msPageHandlerFactory.GetHandler(context, requestType, requestPath, physicalPath);
            }

            return ActionHandler.CreateHandler(vkInfo);
        }

        void IHttpHandlerFactory.ReleaseHandler(IHttpHandler handler)
        {
        }
    }

}
