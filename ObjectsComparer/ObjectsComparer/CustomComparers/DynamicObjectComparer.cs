using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class DynamicObjectComparer : AbstractDynamicObjectsComprer<DynamicObject>
    {
        private class FakeGetMemberBinder : GetMemberBinder
        {
            public FakeGetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotSupportedException();
            }
        }

        public DynamicObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(DynamicObject));
        }

        public override bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return false;
        }

        public override bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }

        protected override IList<string> GetProperties(DynamicObject obj)
        {
            return obj.GetDynamicMemberNames().ToList();
        }

        protected override bool TryGetMemberValue(DynamicObject obj, string propertyName, out object value)
        {
            var getBinder = new FakeGetMemberBinder(propertyName, false);

            return obj.TryGetMember(getBinder, out value);
        }
    }
}
