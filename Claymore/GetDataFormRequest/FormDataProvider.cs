using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace Claymore
{
    internal class FormDataProvider : IActionParametersProvider
    {
        #region IActionParametersProvider 成员

        public object[] GetParameters(System.Web.HttpRequest request, ActionDescription action)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (action == null)
                throw new ArgumentNullException("action");
            object[] parameters = new object[action.Parameters.Length];

            for (int i = 0; i < action.Parameters.Length; i++)
            {
                ParameterInfo p = action.Parameters[i];

                if (p.IsOut)
                    continue;

                if (p.ParameterType == typeof(void))
                    continue;

                if (p.ParameterType == typeof(NameValueCollection))
                {
                    if (string.Compare(p.Name, "Form", StringComparison.OrdinalIgnoreCase) == 0)
                        parameters[i] = request.Form;
                    else if (string.Compare(p.Name, "QueryString", StringComparison.OrdinalIgnoreCase) == 0)
                        parameters[i] = request.QueryString;
                    else if (string.Compare(p.Name, "Headers", StringComparison.OrdinalIgnoreCase) == 0)
                        parameters[i] = request.Headers;
                    else if (string.Compare(p.Name, "ServerVariables", StringComparison.OrdinalIgnoreCase) == 0)
                        parameters[i] = request.ServerVariables;
                }
                else
                {
                    Type paramterType = TypeExtensions.GetRealType(p.ParameterType);
                   
                    if (TypeExtensions.IsSupportableType(paramterType)) 
                    {
                        object val = ModelHelper.GetValueByNameAndTypeFrommRequest(request, p.Name, paramterType, null);
                        if (val != null)
                            parameters[i] = val;
                        else
                        {
                            if (p.ParameterType.IsValueType && TypeExtensions.IsNullableType( p.ParameterType) == false)
                                throw new ArgumentException("未能找到指定的参数值：" + p.Name);
                        }
                    }
                    else
                    {
                        object item = ReflectionExtensions.FastNew(paramterType);
                        ModelHelper.FillModel(request, item, p.Name);
                        parameters[i] = item;
                    }
                }
            }

            return parameters;
        }

        #endregion
    }
}
