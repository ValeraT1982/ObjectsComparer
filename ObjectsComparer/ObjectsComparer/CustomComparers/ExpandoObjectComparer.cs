using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class ExpandoObjectComparer : AbstractDynamicObjectsComprer<ExpandoObject>
    {
        public ExpandoObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(ExpandoObject)) || 
                   (obj1 != null && obj2 != null && type == typeof(object) && obj1.GetType().InheritsFrom(typeof(ExpandoObject)) && obj2.GetType().InheritsFrom(typeof(ExpandoObject)));
        }

        public override bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return obj1 == null || obj2 == null;
        }

        public override bool SkipMember(Type type, MemberInfo member) => false;

        protected override IList<string> GetProperties(ExpandoObject obj)
        {
            return ((IDictionary<string, object>) obj)?.Keys.ToList() ?? new List<string>();
        }

        protected override bool TryGetMember(ExpandoObject obj, string propertyName, out MemberInfo value)
        {
            value = null;
            return false;
        }

        protected override bool TryGetMemberValue(ExpandoObject obj, string propertyName, out object value)
        {
            if (obj != null)
            {
                return ((IDictionary<string, object>) obj).TryGetValue(propertyName, out value);
            }

            value = null;

            return false;

        }
    }
}
