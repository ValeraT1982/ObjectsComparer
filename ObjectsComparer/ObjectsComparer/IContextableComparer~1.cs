using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Generic comparer that accept comparison context.
    /// </summary>
    public interface IContextableComparer<T>
    {
        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, IComparisionContext comparisionContext);
    }
}
