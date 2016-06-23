using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ActionAttribute : Attribute
    {
        /// <summary>
        /// 允许哪些访问动词，与web.config中的httpHanlder的配置意义一致。
        /// </summary>
        public string Verb { get; set; }


        internal bool AllowExecute(string httpMethod)
        {
            if (string.IsNullOrEmpty(Verb) || Verb == "*")
            {
                return true;
            }
            else
            {
                List<string> verbArray = new List<string>(Verb.Split(','));
                return verbArray.Contains(httpMethod);
            }
        }
    }
}
