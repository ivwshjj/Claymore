using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    internal static class TypeExtensions
    {   
        public static bool IsSupportableType(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(decimal)
                || type == typeof(Guid)
                || type.IsEnum
                || type == typeof(string[]);
        }

        public static Type GetRealType(Type type)
        {
            if (type.IsGenericType)
                return Nullable.GetUnderlyingType(type) ?? type;
            else
                return type;
        }


        public static bool IsNullableType(Type nullableType)
        {
            if (nullableType.IsGenericType
                && nullableType.IsGenericTypeDefinition == false
                && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;

            return false;
        }

    }
}
