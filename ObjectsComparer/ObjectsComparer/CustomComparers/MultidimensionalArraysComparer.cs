using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class MultidimensionalArraysComparer : AbstractEnumerablesComparer
    {
        public MultidimensionalArraysComparer(ComparisonSettings settings, BaseComparer parentComparer,
            IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, new ComparisonContext());
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, IComparisonContext comparisonContext)
        {
            if (obj1 == null && obj2 == null)
            {
                yield break;
            }

            var typeInfo = (obj1 ?? obj2).GetType().GetTypeInfo();
            var enumerablesComparerType = typeof(MultidimensionalArrayComparer<>).MakeGenericType(typeInfo.GetElementType());
            var comparer = (IComparer)Activator.CreateInstance(enumerablesComparerType, Settings, this, Factory);

            foreach (var difference in comparer.CalculateDifferences(type, obj1, obj2, comparisonContext))
            {
                yield return difference;
            }
        }

        public override bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.GetTypeInfo().IsArray && type.GetTypeInfo().GetArrayRank() > 1;
        }

        public override bool SkipMember(Type type, MemberInfo member)
        {
            if (base.SkipMember(type, member))
            {
                return true;
            }

            if (!type.IsArray)
            {
                return false;
            }

            Array array = new int[0];
            return member.Name == PropertyHelper.GetMemberInfo(() => array.Length).Name;
        }
    }
}