using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class TypesComparer : AbstractComparer, IComparerWithCondition
    {
        public TypesComparer(ComparisonSettings settings, BaseComparer parentComparer,
            IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            var group = type.GetGroupName(Settings);

            if (obj1 == null && obj2 == null)
            {
                yield break;
            }

            if (obj1?.GetType().InheritsFrom(typeof(Type)) == false)
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (obj2?.GetType().InheritsFrom(typeof(Type)) == false)
            {
                throw new ArgumentException(nameof(obj2));
            }

            var type1Str = obj1?.ToString();
            var type2Str = obj2?.ToString();

            if (type1Str != type2Str)
            {
                yield return new Difference(group, string.Empty, type1Str, type2Str);
            }
        }

        public bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(Type));
        }

        public bool IsStopComparison(Type type, object obj1, object obj2) => true;

        public bool SkipMember(Type type, MemberInfo member) => true;
    }
}