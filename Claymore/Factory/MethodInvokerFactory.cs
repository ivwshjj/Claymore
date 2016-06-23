using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Compilation;

namespace Claymore
{
    public interface IInvokeMethod
    {
        object Invoke(object target, object[] parameters);
    }

    internal interface IBindMethod
    {
        void BindMethod(MethodInfo method);
    }

    internal static class MethodInvokerFactory
    {
        private static readonly Hashtable s_dict = Hashtable.Synchronized(new Hashtable(4096));

        private static readonly Dictionary<string, Type> s_genericTypeDefinitions = new Dictionary<string, Type>();

        static MethodInvokerFactory()
        {
            Type reflectMethodBase = typeof(ReflectMethodBase<>).GetGenericTypeDefinition();
           
            foreach (Type t in typeof(MethodInvokerFactory).Assembly.GetExportedTypes())
            {
                if (t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == reflectMethodBase)
                {
                    s_genericTypeDefinitions.Add(t.Name, t);
                }
            }
        }

        internal static IInvokeMethod GetMethodInvokerWrapper(MethodInfo methodInfo)
        {
            IInvokeMethod method = (IInvokeMethod)s_dict[methodInfo];
            if (method == null)
            {
                method = CreateMethodInvokerWrapper(methodInfo);
                s_dict[methodInfo] = method;
            }

            return method;
        }
       
        public static IInvokeMethod CreateMethodInvokerWrapper(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            ParameterInfo[] pameters = method.GetParameters();

            string key = null;
            if (method.ReturnType == typeof(void))
            {
                if (method.IsStatic)
                {
                    if (pameters.Length == 0)
                        key = "StaticActionWrapper";
                    else
                        key = "StaticActionWrapper`" + pameters.Length.ToString();
                }
                else
                    key = "ActionWrapper`" + (pameters.Length + 1).ToString();
            }
            else
            {
                if (method.IsStatic)
                    key = "StaticFunctionWrapper`" + (pameters.Length + 1).ToString();
                else
                    key = "FunctionWrapper`" + (pameters.Length + 2).ToString();
            }

            Type genericTypeDefinition;
            if (s_genericTypeDefinitions.TryGetValue(key, out genericTypeDefinition) == false)
            {
                return new CommonInvokerWrapper(method);
            }

            Type instanceType = null;
            if (genericTypeDefinition.IsGenericTypeDefinition)
            {
                List<Type> list = new List<Type>(pameters.Length + 2);
                if (method.IsStatic == false)
                    list.Add(method.DeclaringType);

                for (int i = 0; i < pameters.Length; i++)
                    list.Add(pameters[i].ParameterType);

                if (method.ReturnType != typeof(void))
                    list.Add(method.ReturnType);

                instanceType = genericTypeDefinition.MakeGenericType(list.ToArray());
            }
            else
            {
                instanceType = genericTypeDefinition;
            }
            IInvokeMethod instance = (IInvokeMethod)Activator.CreateInstance(instanceType);

            IBindMethod binder = instance as IBindMethod;
            if (binder != null)
                binder.BindMethod(method);

            return instance;
        }
    }

    internal class CommonInvokerWrapper : IInvokeMethod
    {
        private MethodInfo _method;

        public CommonInvokerWrapper(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }

        public object Invoke(object target, object[] parameters)
        {
            if (_method.ReturnType == typeof(void))
            {
                _method.Invoke(target, parameters);
                return null;
            }

            return _method.Invoke(target, parameters);
        }
    }
}
