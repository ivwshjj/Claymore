using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Claymore
{
    internal static class ReflectionExtensions
    {
        
        private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
        
        private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));

        private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(4096));

        public static object FastNew(Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");

            CtorDelegate ctor = (CtorDelegate)s_methodDict[instanceType];
            if (ctor == null)
            {
                ConstructorInfo ctorInfo = instanceType.GetConstructor(Type.EmptyTypes);
                ctor = DynamicMethodFactory.CreateConstructor(ctorInfo);
                s_methodDict[instanceType] = ctor;
            }

            return ctor();
        }

        public static object FastGetFieldValue(FieldInfo fieldInfo, object obj)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            GetValueDelegate getter = (GetValueDelegate)s_getterDict[fieldInfo];
            if (getter == null)
            {
                getter = DynamicMethodFactory.CreateFieldGetter(fieldInfo);
                s_getterDict[fieldInfo] = getter;
            }

            return getter(obj);
        }

        public static void FastSetField(FieldInfo fieldInfo, object obj, object value)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            SetValueDelegate setter = (SetValueDelegate)s_setterDict[fieldInfo];
            if (setter == null)
            {
                setter = DynamicMethodFactory.CreateFieldSetter(fieldInfo);
                s_setterDict[fieldInfo] = setter;
            }

            setter(obj, value);
        }

        public static object FastGetPropertyValue(PropertyInfo propertyInfo, object obj)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            GetValueDelegate getter = (GetValueDelegate)s_getterDict[propertyInfo];
            if (getter == null)
            {
                getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
                s_getterDict[propertyInfo] = getter;
            }

            return getter(obj);
        }

        public static void FastSetProperty(PropertyInfo propertyInfo, object obj, object value)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            SetValueDelegate setter = (SetValueDelegate)s_setterDict[propertyInfo];
            if (setter == null)
            {
                setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
                s_setterDict[propertyInfo] = setter;
            }

            setter(obj, value);
        }

        public static object FastInvokeMethod(MethodInfo methodInfo, object obj, params object[] parameters)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            MethodDelegate invoker = (MethodDelegate)s_methodDict[methodInfo];
            if (invoker == null)
            {
                invoker = DynamicMethodFactory.CreateMethod(methodInfo);
                s_methodDict[methodInfo] = invoker;
            }

            return invoker(obj, parameters);
        }
    }

    internal static class ReflectionExtensions2
    {
        internal static T GetMyAttribute<T>(MemberInfo m, bool inherit) where T : Attribute
        {
            T[] array = m.GetCustomAttributes(typeof(T), inherit) as T[];

            if (array.Length == 1)
                return array[0];

            if (array.Length > 1)
                throw new InvalidProgramException(string.Format("方法 {0} 不能同时指定多次 [{1}]。", m.Name, typeof(T)));

            return default(T);
        }

        internal static T GetMyAttribute<T>(MemberInfo m) where T : Attribute
        {
            return GetMyAttribute<T>(m, false);
        }


        internal static T[] GetMyAttributes<T>(MemberInfo m, bool inherit) where T : Attribute
        {
            return m.GetCustomAttributes(typeof(T), inherit) as T[];
        }

        internal static T[] GetMyAttributes<T>(MemberInfo m) where T : Attribute
        {
            return m.GetCustomAttributes(typeof(T), false) as T[];
        }
    }
}
