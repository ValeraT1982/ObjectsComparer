using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Comparer accepting <see cref="ComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer<T>
    {
        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, IComparisonContext comparisonContext);
    }
}
