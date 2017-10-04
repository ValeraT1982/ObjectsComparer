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
            return type.InheritsFrom(typeof(ExpandoObject));
        }

        public override bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return false;
        }

        public override bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }

        protected override IList<string> GetProperties(ExpandoObject obj)
        {
            return ((IDictionary<string, object>)obj).Keys.ToList();
        }

        protected override bool TryGetMemberValue(ExpandoObject obj, string propertyName, out object value)
        {
            return ((IDictionary<string, object>)obj).TryGetValue(propertyName, out value);
        }
    }
}
