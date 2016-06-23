using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    /// <summary>
    /// 用于访问当前请求上下文的工具类。
    /// </summary>
    internal static class HttpContextHelper
    {
       
        /// <summary>
        /// return HttpRuntime.AppDomainAppPath;
        /// </summary>
        public static string AppRootPath
        {
            get
            {   
                return HttpRuntime.AppDomainAppPath;
            }
           
        }


        /// <summary>
        /// return HttpContext.Current.Request.FilePath;
        /// </summary>
        public static string RequestFilePath
        {
            get
            {
                return HttpContext.Current.Request.FilePath;
            }
            
        }


        /// <summary>
        /// return HttpContext.Current.Request.RawUrl;
        /// </summary>
        public static string RequestRawUrl
        {
            get
            {
                return HttpContext.Current.Request.RawUrl;
            }
            
        }


        /// <summary>
        /// return HttpContext.Current.User.Identity.Name;
        /// </summary>
        public static string UserIdentityName
        {
            get
            {
                if (HttpContext.Current.Request.IsAuthenticated == false)
                    return null;

                return HttpContext.Current.User.Identity.Name;
            }
           
        }



        // 如果还需要访问更多的HttpContext信息，也可以采用下面的方法。请自行完成。

        //public static HttpContextBase Current
        //{
        //    get { }
        //    set { }
        //}
    }
}
