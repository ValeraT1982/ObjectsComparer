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

            if (t1 == t2)
            {
                return true;
            }

            if (t1.IsGenericType && t1.GetGenericTypeDefinition() == t2)
            {
                return true;
            }

            if (t1.GetInterfaces().Any(i => (i.IsGenericType && i.GetGenericTypeDefinition() == t2) || i == t2))
            {
                return true;
            }

            if (t1.BaseType != null &&
                InheritsFrom(t1.BaseType, t2))
            {
                return true;
            }

            return false;
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
