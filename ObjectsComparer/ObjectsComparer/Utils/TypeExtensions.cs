using System;
using System.Linq;

namespace ObjectsComparer.Utils
{
    public static class TypeExtensions
    {
        public static bool InheritsFrom(this Type t1, Type t2)
        {
            if (null == t1 || null == t2)
            {
                return false;
            }

            if (null != t1.BaseType &&
                t1.BaseType.IsGenericType &&
                t1.BaseType.GetGenericTypeDefinition() == t2)
            {
                return true;
            }

            if (InheritsFrom(t1.BaseType, t2))
            {
                return true;
            }

            return t2.IsAssignableFrom(t1) && t1 != t2 || t1.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == t2);
        }

        public static bool IsComparable(this Type type)
        {
            if (type.IsPrimitive ||
                type.IsEnum ||
                type.InheritsFrom(typeof(IComparable)) ||
                type.InheritsFrom(typeof(IComparable<>)))
            {
                return true;
            }

            return false;
        }
    }
}
