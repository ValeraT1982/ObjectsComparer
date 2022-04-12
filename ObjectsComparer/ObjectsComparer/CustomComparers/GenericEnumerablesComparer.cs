using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ObjectsComparer.DifferenceTreeExtensions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class GenericEnumerablesComparer : AbstractEnumerablesComparer
    {
        public GenericEnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer,
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

            Type elementType;

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = typeInfo.GetElementType();
            }
            else
            {
                elementType = typeInfo.GetInterfaces()
                    .Where(
                        i =>
                            i.GetTypeInfo().IsGenericType &&
                            i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(i => i.GetTypeInfo().GetGenericArguments()[0])
                    .First();
            }

            var enumerablesComparerType = typeof(EnumerablesComparer<>).MakeGenericType(elementType);
            var comparer = (IComparer)Activator.CreateInstance(enumerablesComparerType, Settings, this, Factory);

            foreach (var difference in comparer.CalculateDifferences(type, obj1, obj2, comparisonContext))
            {
                yield return difference;
            }
        }

        public override bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(IEnumerable<>));
        }

        public override bool SkipMember(Type type, MemberInfo member)
        {
            if (base.SkipMember(type, member))
            {
                return true;
            }

            if (type.InheritsFrom(typeof(ICollection<>)) &&
                member.Name == PropertyHelper.GetMemberInfo(() => new Collection<string>().Count).Name)
            {
                return true;
            }

            if (!type.InheritsFrom(typeof(IDictionary<,>)))
            {
                return false;
            }

            return member.Name == PropertyHelper.GetMemberInfo(() => new Dictionary<object, object>().Values).Name ||
                   member.Name == PropertyHelper.GetMemberInfo(() => new Dictionary<object, object>().Keys).Name;
        }
    }
}