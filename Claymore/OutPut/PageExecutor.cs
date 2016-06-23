using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace Claymore
{
    internal static class PageExecutor
    {
        internal static readonly Type MyPageViewOpenType = typeof(MyPageView<>);

        public static void TrySetPageModel(HttpContext context)
        {
            if (context == null || context.Handler == null)
                return;

            IHttpHandler handler = context.Handler;

            Type handlerType = handler.GetType().BaseType;
            if (handlerType.IsGenericType &&
                handlerType.GetGenericTypeDefinition() == MyPageViewOpenType)
            {
                InvokeInfo vkInfo = ReflectionHelper.GetActionInvokeInfo(context.Request.FilePath);
                if (vkInfo == null)
                    return;

                object model = ActionExecutor.ExecuteActionInternal(context, vkInfo);

                SetPageModel(context.Handler, model);
            }
        }


        private static void SetPageModel(IHttpHandler handler, object model)
        {
            if (handler == null)
                return;

            if (model != null)
            {
                MyBasePage page = handler as MyBasePage;
                if (page != null)
                    page.SetModel(model);
            }
        }

        public static string Render(HttpContext context, string pageVirtualPath, object model)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(pageVirtualPath))
                throw new ArgumentNullException("pageVirtualPath");


            Page handler = BuildManager.CreateInstanceFromVirtualPath(
                                            pageVirtualPath, typeof(object)) as Page;
            if (handler == null)
                throw new InvalidOperationException(
                    string.Format("指定的路径 {0} 不是一个有效的页面。", pageVirtualPath));


            SetPageModel(handler, model);

            StringWriter output = new StringWriter();
            context.Server.Execute(handler, output, false);
            return output.ToString();
        }

        [Obsolete("这个方法即将被移除，请调用ResponseWriter中的WritePage方法来代替。")]
        public static void ResponseWrite(string pageVirtualPath, object model, bool flush)
        {
            ResponseWriter.WritePage(pageVirtualPath, model, flush);
        }


    }
}
