using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PageUrlAttribute : Attribute
    {
        public string Url { get; set; }
    }
}
