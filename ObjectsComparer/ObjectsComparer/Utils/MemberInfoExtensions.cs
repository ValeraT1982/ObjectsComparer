using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer.Utils
{
    internal static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            throw new Exception("Unsupported Type");
        }

        public static object GetMemberValue(this MemberInfo memberInfo, object obj)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                try
                {
                    return propertyInfo.GetValue(obj);
                }
                catch
                {
                    return $"Unable to get value of property {memberInfo.Name} of type {memberInfo.DeclaringType}";
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            throw new Exception("Unsupported Type");
        }

        public static string GetMethodName<T>(Expression<Action<T>> expression)
        {
            return ((MethodCallExpression)expression.Body).Method.Name;
        }
    }
}
