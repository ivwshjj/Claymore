using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Claymore.Helper
{
    /// <summary>  
    /// 节点枚举  
    /// </summary>  
    internal enum NodeType
    {
        /// <summary>  
        /// 标识数组  
        /// </summary>  
        IsArray
        ,
        /// <summary>  
        /// 标识对象  
        /// </summary>  
        IsObject
            ,
        /// <summary>  
        /// 标识元数据  
        /// </summary>  
        IsOriginal

            ,
        /// <summary>  
        /// 未知格式  
        /// </summary>  
        Undefined
    }

    //描述Json节点  
    internal class JsonNode
    {
        public NodeType NodeType;
        public List<JsonNode> List;
        public Dictionary<string, JsonNode> DicObject;
        public string Value;
    }

    /// <summary>  
    /// json 字符串对象化  
    /// 引用至 http://www.cnblogs.com/a_bu/archive/2012/10/10/2719168.html
    /// </summary>  
    internal class JsonHelper
    {
        static string regTxt = "({0}[^{0}{1}]*(((?'Open'{0})[^{0}{1}]*)+((?'-Open'{1})[^{0}{1}]*)+)*(?(Open)(?!)){1})";

        //匹配字符串(单双引号范围)  
        static string regKeyValue = "({0}.{1}?(?<!\\\\){0})";  //判断是否包含单,双引号  

        //匹配元数据(不包含对象,数组)  
        static string regOriginalValue = string.Format("({0}|{1}|{2})", string.Format(regKeyValue, "'", "*"), string.Format(regKeyValue, "\"", "*"), "\\w+");

        //匹配value  (包含对象数组)  
        static string regValue = string.Format("({0}|{1}|{2})", regOriginalValue  //字符  
               , string.Format(regTxt, "\\[", "\\]"), string.Format(regTxt, "\\{", "\\}"));

        //匹配键值对  
        static string regKeyValuePair = string.Format("\\s*(?<key>{0}|{1}|{2})\\s*:\\s*(?<value>{3})\\s*"
            , string.Format(regKeyValue, "'", "+"), string.Format(regKeyValue, "\"", "+"), "([^ :,]+)" //匹配key  
            , regValue);     //匹配value    

        /// <summary>  
        /// 判断是否是对象  
        /// </summary>  
        static Regex RegJsonStrack1 = new Regex(string.Format("^\\{0}(({2})(,(?=({2})))?)+\\{1}$", "{", "}", regKeyValuePair), RegexOptions.Compiled);

        /// <summary>  
        /// 判断是否是序列  
        /// </summary>  
        static Regex RegJsonStrack2 = new Regex(string.Format("^\\[(({0})(,(?=({0})))?)+\\]$", regValue), RegexOptions.Compiled);

        /// <summary>  
        /// 判断键值对  
        /// </summary>  
        static Regex RegJsonStrack3 = new Regex(regKeyValuePair, RegexOptions.Compiled);

        //匹配value  
        static Regex RegJsonStrack4 = new Regex(regValue, RegexOptions.Compiled);

        //匹配元数据  
        static Regex RegJsonStrack6 = new Regex(string.Format("^{0}$", regOriginalValue), RegexOptions.Compiled);

        //移除两端[] , {}  
        static Regex RegJsonRemoveBlank = new Regex("(^\\s*[\\[\\{'\"]\\s*)|(\\s*[\\]\\}'\"]\\s*$)", RegexOptions.Compiled);




        string JsonTxt;
        public JsonHelper(string json)
        {
            //去掉换行符  
            json = Regex.Replace(json, "[\r\n]", "");

            JsonTxt = json;
        }

        /// <summary>  
        /// 判断节点内型  
        /// </summary>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public NodeType MeasureType(string json)
        {
            if (RegJsonStrack1.IsMatch(json))
            {
                return NodeType.IsObject;
            }

            if (RegJsonStrack2.IsMatch(json))
            {
                return NodeType.IsArray;
            }

            if (RegJsonStrack6.IsMatch(json))
            {
                return NodeType.IsOriginal;
            }

            return NodeType.Undefined;

        }

        /// <summary>  
        /// json 字符串序列化为对象  
        /// </summary>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public JsonNode SerializationJsonNodeToObject()
        {
            return SerializationJsonNodeToObject(JsonTxt);
        }

        /// <summary>  
        /// json 字符串序列化为对象  
        /// </summary>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public JsonNode SerializationJsonNodeToObject(string json)
        {
            json = json.Trim();
            NodeType nodetype = MeasureType(json);
            if (nodetype == NodeType.Undefined)
            {
                throw new Exception("未知格式Json: " + json);
            }

            JsonNode newNode = new JsonNode();
            newNode.NodeType = nodetype;

            if (nodetype == NodeType.IsArray)
            {
                json = RegJsonRemoveBlank.Replace(json, "");
                MatchCollection matches = RegJsonStrack4.Matches(json);
                newNode.List = new List<JsonNode>();
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        newNode.List.Add(SerializationJsonNodeToObject(match.Value));
                    }
                }
            }
            else if (nodetype == NodeType.IsObject)
            {
                json = RegJsonRemoveBlank.Replace(json, "");
                MatchCollection matches = RegJsonStrack3.Matches(json);
                newNode.DicObject = new Dictionary<string, JsonNode>();
                string key;
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        key = RegJsonRemoveBlank.Replace(match.Groups["key"].Value, "");
                        if (newNode.DicObject.ContainsKey(key))
                        {
                            throw new Exception("json 数据中包含重复键, json:" + json);
                        }
                        newNode.DicObject.Add(key, SerializationJsonNodeToObject(match.Groups["value"].Value));
                    }
                }
            }
            else if (nodetype == NodeType.IsOriginal)
            {
                newNode.Value = RegJsonRemoveBlank.Replace(json, "").Replace("\\r\\n", "\r\n");
            }

            return newNode;
        }
    }  
}
