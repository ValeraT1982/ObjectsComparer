using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Generic comparer that accept comparison's process context, see <see cref="IComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer<T>
    {
        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, IComparisonContext comparisonContext);
    }
}
