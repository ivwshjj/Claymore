using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Claymore.Extensions;

namespace Claymore
{
    /// <summary>
    /// 描述类型字段/属性 的基本信息类
    /// </summary>
    internal abstract class DataMember
    {
        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object val);
        /// <summary>
        /// 字段/属性 的类型
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        ///  字段/属性 的名称
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        ///  字段/属性 是否区分大小写,默认不区分
        /// </summary>
        public abstract bool Ignore { get; }
    }

    /// <summary>
    /// 类属性的具体描述类
    /// </summary>
    internal sealed class PropertyMember : DataMember
    {
        private bool _ignore;
        private PropertyInfo _pi;

        public PropertyMember(PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException("pi");
            _pi = pi;
            _ignore = false;
        }

        public override object GetValue(object obj)
        {
            return _pi.FastGetPropertyValue(obj);
        }

        public override void SetValue(object obj, object val)
        {
            _pi.FastSetProperty(obj, val);
        }

        public override Type Type
        {
            get { return _pi.PropertyType; }
        }

        public override string Name
        {
            get { return _pi.Name; }
        }

        public override bool Ignore
        {
            get { return _ignore; }
        }
    }

    /// <summary>
    /// 字段的具体描述类
    /// </summary>
    internal sealed class FieldMember : DataMember
    {
        private bool _ignore;
        private FieldInfo _field;

        public FieldMember(FieldInfo fi)
        {
            if (fi == null)
                throw new ArgumentNullException("fi");
            _field = fi;
            _ignore = false;
        }

        public override object GetValue(object obj)
        {
            return _field.FastGetFieldValue(obj);
        }

        public override void SetValue(object obj, object val)
        {
            _field.FastSetField(obj, val);
        }

        public override Type Type
        {
            get { return _field.FieldType; }
        }

        public override string Name
        {
            get { return _field.Name; }
        }

        public override bool Ignore
        {
            get { return _ignore; }
        }
    }
}
