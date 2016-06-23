using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Claymore
{
    internal static class MethodInfoExtensions
    {
        public static object FastInvoke(MethodInfo methodInfo, object obj, params object[] parameters)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            IInvokeMethod method = MethodInvokerFactory.GetMethodInvokerWrapper(methodInfo);
            return method.Invoke(obj, parameters);
        }
    }
}
