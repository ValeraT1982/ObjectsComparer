using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ObjectsComparer
{
    internal class CompilerGeneratedObjectComparer : AbstractDynamicObjectsComprer<object>
    {
        public CompilerGeneratedObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override bool IsMatch(Type type, object obj1, object obj2)
        {
            return (obj1 != null || obj2 != null) &&
                   (obj1 == null || obj1.GetType().GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null) &&
                   (obj2 == null || obj2.GetType().GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null);
        }

        public override bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return true;
        }

        public override bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }

        protected override IList<string> GetProperties(object obj)
        {
            return obj?.GetType().GetTypeInfo().GetMembers()
                .Where(memberInfo => memberInfo is PropertyInfo)
                .Select(memberInfo => memberInfo.Name)
                .Distinct()
                .ToList() ?? new List<string>();
        }

        protected override bool TryGetMemberValue(object obj, string propertyName, out object value)
        {
            value = null;
            if (obj == null)
            {
                return false;
            }

            var propertyInfo = obj.GetType().GetTypeInfo().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return false;
            }

            value = propertyInfo.GetValue(obj);

            return true;

        }

        protected override bool TryGetMember(object obj, string propertyName, out MemberInfo value)
        {
            value = null;

            if (obj == null)
            {
                return false;
            }

            value = obj.GetType().GetTypeInfo().GetProperty(propertyName);

            if (value == null)
            {
                return false;
            }

            return false;
        }
    }
}
