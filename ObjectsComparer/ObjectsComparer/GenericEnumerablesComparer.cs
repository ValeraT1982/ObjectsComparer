using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class GenericEnumerablesComparer : AbstractComparer, IComparerWithCondition
    {
        public GenericEnumerablesComparer(ComparisonSettings settings, IComparer parentComparer,
            IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                yield break;
            }

            var typeInfo = (obj1 ?? obj2).GetType().GetTypeInfo();

            Type elementType;

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = typeInfo.GetGenericArguments()[0];
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
            var enumerablesComparer =
                (IComparer) Activator.CreateInstance(enumerablesComparerType, Settings, this, Factory);

            foreach (var difference in enumerablesComparer.CalculateDifferences(obj1, obj2))
            {
                yield return difference;
            }
        }

        public bool IsMatch(Type type)
        {
            return type.InheritsFrom(typeof(IEnumerable<>));
        }

        public bool IsStopComparison(object obj1, object obj2)
        {
            if (Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null)
            {
                return true;
            }

            return false;
        }
    }
}