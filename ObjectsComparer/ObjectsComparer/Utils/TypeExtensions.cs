using System;
using System.Linq;
using System.Reflection;

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

            if (t1.GetTypeInfo().IsGenericType && t1.GetTypeInfo().GetGenericTypeDefinition() == t2)
            {
                return true;
            }

            if (t1.GetTypeInfo().GetInterfaces().Any(i => (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == t2) || i == t2))
            {
                return true;
            }

            if (t1.GetTypeInfo().BaseType != null &&
                InheritsFrom(t1.GetTypeInfo().BaseType, t2))
            {
                return true;
            }

            return false;
        }

        public static bool IsComparable(this Type type)
        {
            if (type.GetTypeInfo().IsPrimitive ||
                type.GetTypeInfo().IsEnum ||
                type.InheritsFrom(typeof(IComparable)) ||
                type.InheritsFrom(typeof(IComparable<>)))
            {
                return true;
            }

            return false;
        }
    }
}
