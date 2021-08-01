using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public static class IComparableExtensions
    {
        public static IEnumerable<Difference> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, IComparisionContext currentContext)
        {
            if (comparer is IContextableComparer contextableComparer)
            {
                return contextableComparer.CalculateDifferences(type, obj1, obj2, currentContext);
            }

            return comparer.CalculateDifferences(type, obj1, obj2);
        }
    }

    
}