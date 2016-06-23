using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    internal static class ActionExecutor
    {
        private static readonly string MyVersion
            = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(ActionExecutor).Assembly.Location).FileVersion;


        private static void SetVersionHeader(HttpContext context)
        {
            context.Response.AppendHeader("X-Claymore-Version", MyVersion);
        }

        internal static void ExecuteAction(HttpContext context, InvokeInfo vkInfo)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (vkInfo == null)
                throw new ArgumentNullException("vkInfo");

            SetVersionHeader(context);

            // 验证请求是否允许访问（身份验证）
            AuthorizeAttribute authorize = vkInfo.GetAuthorize();
            if (authorize != null)
            {
                if (authorize.AuthenticateRequest(context) == false)
                {
                    authorize.RequestResult(context);
                }
            }

            // 调用方法
            object result = ExecuteActionInternal(context, vkInfo);

            // 设置OutputCache
            OutputCacheAttribute outputCache = vkInfo.GetOutputCacheSetting();
            if (outputCache != null)
                outputCache.SetResponseCache(context);
            
            // 处理方法的返回结果
            IActionResult executeResult = result as IActionResult;
            if (executeResult != null)
            {
                executeResult.Ouput(context);
            }
            else
            {
                if (result != null)
                {
                    // 普通类型结果
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(result.ToString());
                }
            }
        }

        internal static object ExecuteActionInternal(HttpContext context, InvokeInfo info)
        {
            // 准备要传给调用方法的参数
            object[] parameters = GetActionCallParameters(context, info.Action);

            // 调用方法
            if (info.Action.HasReturn)
            {
                return MethodInfoExtensions.FastInvoke(info.Action.MethodInfo, info.Instance, parameters);
            }

            else
            {
                MethodInfoExtensions.FastInvoke(info.Action.MethodInfo, info.Instance, parameters);
                return null;
            }
        }


        private static object[] GetActionCallParameters(HttpContext context, ActionDescription action)
        {
            if (action.Parameters == null || action.Parameters.Length == 0)
                return null;

            IActionParametersProvider provider = ActionParametersProviderFactory.CreateActionParametersProvider(context.Request);
            return provider.GetParameters(context.Request, action);
        }

    }
}
