using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Claymore.Extensions;
using Claymore.Helper;

namespace Claymore
{
    internal class XmlDataProvider : IActionParametersProvider
    {
        public object[] GetParameters(HttpRequest request, ActionDescription action)
        {
            if (action.Parameters.Length == 1)
            {
                object value = GetObjectFromRequest(request, action);
                return new object[1] { value };
            }
            else
                return GetMultiObjectsFormRequest(request, action);
        }


        public object GetObjectFromRequest(HttpRequest request, ActionDescription action)
        {
            Type destType = TypeExtensions.GetRealType(action.Parameters[0].ParameterType);

            XmlSerializer mySerializer = new XmlSerializer(destType);

            request.InputStream.Position = 0;
            StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding);
            return mySerializer.Deserialize(sr);
        }

        public object[] GetMultiObjectsFormRequest(HttpRequest request, ActionDescription action)
        {
            //string xml = request.ReadInputStream();
            request.InputStream.Position = 0;
            StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding);
            string xml = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode root = doc.LastChild;

            //if( root.ChildNodes.Count != action.Parameters.Length )
            //    throw new ArgumentException("客户端提交的数据项与服务端的参数项的数量不匹配。");

            object[] parameters = new object[action.Parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                string name = action.Parameters[i].Name;
                XmlNode node = (from n in root.ChildNodes.Cast<XmlNode>()
                                where string.Compare(n.Name, name, StringComparison.OrdinalIgnoreCase) == 0
                                select n).FirstOrDefault();

                if (node != null)
                {
                    object parameter = null;
                    Type destType = TypeExtensions.GetRealType(action.Parameters[i].ParameterType);

                    if (TypeExtensions.IsSupportableType( destType))
                        parameter = ModelHelper.SafeChangeType(node.InnerText, destType);
                    else
                        parameter = XmlDeserialize(node.OuterXml, destType, request.ContentEncoding);

                    parameters[i] = parameter;
                }
            }

            return parameters;
        }



        public static object XmlDeserialize(string xml, Type destType, Encoding encoding)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(destType);
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return mySerializer.Deserialize(sr);
                }
            }
        }

    }
}
