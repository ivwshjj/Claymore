using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    public enum SessionMode
    {
        /// <summary>
        /// 不支持
        /// </summary>
        NotSupport,
        /// <summary>
        /// 全支持
        /// </summary>
        Support,
        /// <summary>
        /// 仅支持读取
        /// </summary>
        ReadOnly
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SessionModeAttribute : Attribute
    {
        public SessionMode SessionMode { get; private set; }

        public SessionModeAttribute(SessionMode mode)
        {
            this.SessionMode = mode;
        }
    }
}
