using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Comparer accepting <see cref="IComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer
    {
        IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, IComparisonContext comparisonContext);
    }
}