using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    public abstract class BaseActionHandlerFactory : IHttpHandlerFactory
    {
        public abstract ControllerActionPair ParseUrl(HttpContext context, string path);

        /// <summary>
        /// 是不是服务类型的判断
        /// </summary>
        /// <param name="type">类型信息</param>
        /// <returns></returns>
        public abstract bool TypeIsService(Type type);

        public IHttpHandler GetHandler(HttpContext context,
                            string requestType, string virtualPath, string physicalPath)
        {
            string vPath = UrlHelper.GetRealVirtualPath(context, context.Request.Path);

            ControllerActionPair pair = ParseUrl(context, vPath);
            if (pair == null)
                ExceptionHelper.Throw404Exception(context);

            InvokeInfo vkInfo = ReflectionHelper.GetActionInvokeInfo(pair, context.Request);
            if (vkInfo == null)
                ExceptionHelper.Throw404Exception(context);

            return ActionHandler.CreateHandler(vkInfo);
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}
