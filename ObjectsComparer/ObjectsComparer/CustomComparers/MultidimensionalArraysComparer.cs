using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.DifferenceTreeExtensions;
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
            return BuildDifferenceTree(type, obj1, obj2, ComparisonContextProvider.CreateImplicitRootContext(Settings))
                .Select(differenceLocation => differenceLocation.Difference);
        }

        public override IEnumerable<DifferenceLocation> BuildDifferenceTree(Type type, object obj1, object obj2, IDifferenceTreeNode comparisonContext)
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