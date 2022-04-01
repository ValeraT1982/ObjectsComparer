using System;
using System.Collections.Generic;

namespace ObjectsComparer.ContextExtensions
{
    /// <summary>
    /// Comparer accepting <see cref="IComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer<T>
    {
        IEnumerable<DifferenceLocation> CalculateDifferences(T obj1, T obj2, IComparisonContext comparisonContext);
    }
}
