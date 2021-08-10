using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Generic comparer that accept comparison's process context, see <see cref="ComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer<T>
    {
        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, ComparisonContext comparisonContext);
    }
}
