using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Claymore.Helper
{
    public class MyJsonHelper
    {
        public static T DeserializeObject<T>(string json) where T : class,new()
        {
            if (string.IsNullOrEmpty(json))
                return null;

            JsonHelper jsonhelper = new JsonHelper(json);
            JsonNode jn = jsonhelper.SerializationJsonNodeToObject();
            T t = new T();
            ModelDescripton modelDescripton = ModelHelper.GetModelDescripton(t.GetType());
            if (t.GetType().Name == typeof(List<>).Name)
            {
               SetValue(modelDescripton.Fields[0], jn, t);
               return t;
            }
            JsonNode jval;
            foreach (var i in modelDescripton.Fields)
            { 
                jn.DicObject.TryGetValue(i.Name,out jval);
                SetValue(i,jval,t);
            }
            return t;
        }

        public static string SerializeObject(object obj)
        {
            if (obj == null)
                return string.Empty;
            ModelDescripton modelDescripton = ModelHelper.GetModelDescripton(obj.GetType());
            if (obj.GetType().Name == typeof(List<>).Name)
            {
                return GetListJson(modelDescripton.Fields[0], obj).ToString();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            
            foreach (var i in modelDescripton.Fields) {
                sb.AppendFormat("\"{0}\":{1},", i.Name, GetValue(i, obj));
            }
            sb.Length -= 1;
            sb.Append("}");
            return sb.ToString();
        }

        private static string GetValue(DataMember dm,object obj) {
            if (TypeExtensions.IsSupportableType(dm.Type) ||
                TypeExtensions.IsSupportableType(TypeExtensions.GetRealType(dm.Type)))
                return string.Format("\"{0}\"", dm.GetValue(obj));
           
            if (dm.Type.Name == typeof(List<>).Name)
            {
                return GetListJson(dm,obj);
            }
            ModelDescripton modelDescripton = ModelHelper.GetModelDescripton(dm.Type);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var i in modelDescripton.Fields)
            {
                sb.AppendFormat("\"{0}\":{1},", i.Name, GetValue(i,obj));
            }
            sb.Length -= 1;
            sb.Append("}");
            return sb.ToString();
        }
       
        private static string GetListJson(DataMember dm,object obj)
        {
            Type[] t = obj.GetType().GetGenericArguments();
            var temp = t;
            if (t.Length==0)
            {
                t = dm.Type.GetGenericArguments();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            /*如果是obj是 List<T> 且中的T是简单的基本类型*/
            if (TypeExtensions.IsSupportableType(t[0]) && Object.ReferenceEquals(t, temp))
            {  
                var target = obj as IList;

                foreach (var item in target)
                {
                    sb.AppendFormat("{0},",item);
                }
                sb.Length -= 1;
                sb.Append("]");
                return sb.ToString();
            }
            /*如果List<T>是obj中的某个属性,且T是简单的基本类型*/
            else if (TypeExtensions.IsSupportableType(t[0]))
            { 
                var target = dm.GetValue(obj) as IList;
                foreach (var item in target)
                {
                    sb.AppendFormat("{0},", item);
                }
                sb.Length -= 1;
                sb.Append("]");
                return sb.ToString();
            }
            else
            {
                /*如果List<T>中的T是一个自定义的类型*/
                ModelDescripton modelDescripton = ModelHelper.GetModelDescripton(t[0]);
                var target = dm.GetValue(obj) as IList;
                foreach (var item in target)
                {
                    sb.Append("{");
                    foreach(var i in modelDescripton.Fields){
                        sb.AppendFormat("\"{0}\" :{1},",i.Name, GetValue(i,item));
                    }
                    sb.Length -= 1;
                    sb.Append("},");
                }
                sb.Length -= 1;
                sb.Append("]");
                return sb.ToString();
                  
            }

        }

        private static void SetValue(DataMember dm, JsonNode node,object obj)
        {
            /*
            if (node == null || string.IsNullOrEmpty(node.Value) || node.List==null || node.List.Count==0)
                return;
             * */
            if ((TypeExtensions.IsSupportableType(dm.Type) ||
               TypeExtensions.IsSupportableType(TypeExtensions.GetRealType(dm.Type))) 
                && string.IsNullOrEmpty(node.Value)==false)
            {
                dm.SetValue(obj, ModelHelper.SafeChangeType(node.Value, dm.Type));
                return;
            }
            Type[] typeArgs = dm.Type.GetGenericArguments();
            //如果是List<T> 且 T是基础类型
            if (dm.Type.Name == typeof(List<>).Name && TypeExtensions.IsSupportableType(typeArgs[0]))
            {
                Type generic = typeof(List<>);
                generic = generic.MakeGenericType(typeArgs);
                //var list = Activator.CreateInstance(generic) as IList;
                var list = ReflectionExtensions.FastNew(generic) as IList;
                foreach (JsonNode n in node.List) {
                    list.Add(ModelHelper.SafeChangeType(n.Value, typeArgs[0]));
                }
                dm.SetValue(obj, list);
                return;
            }
            else if (dm.Type.Name == typeof(List<>).Name)
            {
                Type generic = typeof(List<>);
                generic = generic.MakeGenericType(typeArgs);
                //var list = Activator.CreateInstance(generic) as IList;
                var list = ReflectionExtensions.FastNew(generic) as IList;
                ModelDescripton modelDescripton = ModelHelper.GetModelDescripton(typeArgs[0]);
                JsonNode jn;
                jn = null;
                foreach (var y in node.List)
                {
                    var m = ReflectionExtensions.FastNew(typeArgs[0]);
                    foreach (var i in modelDescripton.Fields)
                    {
                        y.DicObject.TryGetValue(i.Name, out jn);
                        if (jn == null || string.IsNullOrEmpty(jn.Value))
                            continue;
                        SetValue(i, jn, m);
                    }
                    list.Add(m);
                }
                dm.SetValue(obj, list);
                return;
                
            }
            else { 
            
            }
        }
    }
}
