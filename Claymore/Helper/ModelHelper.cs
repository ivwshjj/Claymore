using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Claymore
{
    internal static class ModelHelper
    {
        public static readonly bool IsDebugMode;

        private static Hashtable s_modelTable = Hashtable.Synchronized(
                                            new Hashtable(10240, StringComparer.OrdinalIgnoreCase));
        static ModelHelper()
        {
            CompilationSection configSection =
                        ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            if (configSection != null)
                IsDebugMode = configSection.Debug;
        }

        public static void FillModel(HttpRequest request, object model, string paramName)
        {
            ModelDescripton descripton = GetModelDescripton(model.GetType());
            
            object val = null;
            foreach (DataMember field in descripton.Fields)
            {
                if (field.Ignore)
                    continue;

                val = GetValueByNameAndTypeFrommRequest(
                                    request, field.Name, TypeExtensions.GetRealType(field.Type), paramName);
                if (val != null)
                    field.SetValue(model, val);
            }
        }

        public static ModelDescripton GetModelDescripton(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string key = type.FullName;
            ModelDescripton mm = (ModelDescripton)s_modelTable[key];

            if (mm == null)
            {
                List<DataMember> list = new List<DataMember>();
                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    list.Add(new PropertyMember(p));
                }

                foreach (FieldInfo p in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    list.Add(new FieldMember(p));
                }

                mm = new ModelDescripton { Fields = list };
                s_modelTable[key] = mm;
            }
            return mm;
        }


        private static string[] GetHttpValues(HttpRequest request, string name)
        {
            string[] val = request.QueryString.GetValues(name);
            if (val == null)
                val = request.Form.GetValues(name);

            return val;
        }
     

        private static string[] GetValueFromHttpRequest(HttpRequest request, string name, string parentName)
        {
            string[] val = GetHttpValues(request, name);
            if (val == null)
            {
                // 再试一次。有可能是多个自定义类型，Form表单元素采用变量名做为前缀。
                if (string.IsNullOrEmpty(parentName) == false)
                {
                    val = GetHttpValues(request, parentName + "." + name);
                }
            }
            return val;
        }

        public static object GetValueByNameAndTypeFrommRequest(
                            HttpRequest request, string name, Type type, string parentName)
        {
            MethodInfo stringImplicit = null;

            // 检查是否为不支持的参数类型
            if (TypeExtensions.IsSupportableType(type) == false)
            {

                // 检查是否可以做隐式类型转换
                stringImplicit = GetStringImplicit(type);

                if (stringImplicit == null)
                    return null;
            }

            string[] val = GetValueFromHttpRequest(request, name, parentName);

            if (type == typeof(string[]))
                return val;

            if (val == null || val.Length == 0)
                return null;

            // 还原ASP.NET的默认数据格式
            string str = val.Length == 1 ? val[0] : string.Join(",", val);

            // 可以做隐式类型转换
            if (stringImplicit != null)
            {
                //return stringImplicit.FastInvoke(null, str.Trim());
                return MethodInfoExtensions.FastInvoke(stringImplicit, null, str.Trim());
            }


            return SafeChangeType(str.Trim(), type);
        }



        public static object SafeChangeType(string value, Type conversionType)
        {
            if (conversionType == typeof(string))
                return value;

            if (value == null || value.Length == 0)
            {
                return null;
            }

            try
            {
                if (conversionType == typeof(Guid))
                    return new Guid(value);

                if (conversionType.IsEnum)
                    return Enum.Parse(conversionType, value);

                return Convert.ChangeType(value, conversionType);
            }
            catch
            {
                if (IsDebugMode)
                    throw;			
                else
                {  
                    return null;
                }
            }
        }


        private static MethodInfo GetStringImplicit(Type conversionType)
        {
            MethodInfo m = conversionType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public);

            if (m != null && m.IsStatic && m.IsSpecialName && m.ReturnType == conversionType)
            {
                ParameterInfo[] paras = m.GetParameters();
                if (paras.Length == 1 && paras[0].ParameterType == typeof(string))
                    return m;
            }

            return null;
        }


    }
}
