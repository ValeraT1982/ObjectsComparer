using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public static class IComparableExtensions
    {
        public static IEnumerable<Difference> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, IComparisonContext comparisonContext)
        {
            if (comparer is IContextableComparer contextableComparer)
            {
                return contextableComparer.CalculateDifferences(type, obj1, obj2, comparisonContext);
            }

            return comparer.CalculateDifferences(type, obj1, obj2);
        }

        public static IEnumerable<Difference> CalculateDifferences<T>(this IComparer<T> comparer, T obj1, T obj2, IComparisonContext comparisonContext)
        {
            if (comparer is IContextableComparer<T> contextableComparer)
            {
                return contextableComparer.CalculateDifferences(obj1, obj2, comparisonContext);
            }

            return comparer.CalculateDifferences(obj1, obj2);
        }
    }
}