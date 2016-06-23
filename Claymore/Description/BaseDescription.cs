using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Claymore
{
    internal class BaseDescription
    {
        public OutputCacheAttribute OutputCache { get; protected set; }
        public SessionModeAttribute SessionMode { get; protected set; }
        public AuthorizeAttribute Authorize { get; protected set; }

        protected BaseDescription(MemberInfo m)
        {
            this.OutputCache = ReflectionExtensions2.GetMyAttribute<OutputCacheAttribute>(m);
            this.SessionMode =  ReflectionExtensions2.GetMyAttribute<SessionModeAttribute>(m);
            this.Authorize = ReflectionExtensions2.GetMyAttribute<AuthorizeAttribute>(m, true);
        }
    }
}
