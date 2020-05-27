using System;
using System.Linq;
using System.Reflection;

namespace ObjectsComparer.Utils
{
    internal static class TypeExtensions
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

            if (t1.GetTypeInfo().GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == t2 || i == t2))
            {
                return true;
            }

            return t1.GetTypeInfo().BaseType != null &&
                   InheritsFrom(t1.GetTypeInfo().BaseType, t2);
        }

        public static bool IsComparable(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive ||
                   type.GetTypeInfo().IsEnum ||
                   type.InheritsFrom(typeof(IComparable)) ||
                   type.InheritsFrom(typeof(IComparable<>));
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(t) == null)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        public static string GetGroupName(this Type t, ComparisonSettings settings)
        {
            return t.GetTypeInfo().GetCustomAttribute(settings.GroupNameAttribute)?.ToString() ?? string.Empty;
        }
    }
}
