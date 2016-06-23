using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Claymore
{
    internal abstract class DataMember
    {
        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object val);
       
        public abstract Type Type { get; }
        
        public abstract string Name { get; }
        
        public abstract bool Ignore { get; }
    }

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
            return ReflectionExtensions.FastGetPropertyValue(_pi, obj);
        }

        public override void SetValue(object obj, object val)
        {
            ReflectionExtensions.FastSetProperty(_pi, obj, val);
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
            return ReflectionExtensions.FastGetFieldValue(_field, obj);
        }

        public override void SetValue(object obj, object val)
        {
            _field.SetValue(obj, val);
            ReflectionExtensions.FastSetField(_field, obj, val);
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
