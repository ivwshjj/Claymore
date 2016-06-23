using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    
    internal static class StringExtensions
    {  
        public static bool IsSame(string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string[] SplitTrim(string str, params char[] separator)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            else
            {
                List<string> list = new List<string>(str.Split(separator));
                return list.FindAll(delegate(string x) {
                    return x.Length > 0;
                }).ToArray();
            }
        }


        internal static readonly char[] CommaSeparatorArray = new char[] { ',' };

        public static string ToTitleCase(string text)
        {
            if (text == null || text.Length < 2)
                return text;

            char c = text[0];
            if ((c >= 'a') && (c <= 'z'))
                return ((char)(c - 32)).ToString() + text.Substring(1);
            else
                return text;
        }

    }
}
