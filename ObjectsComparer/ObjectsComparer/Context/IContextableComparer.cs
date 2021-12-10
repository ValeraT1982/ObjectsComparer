using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Comparer accepting <see cref="ComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer
    {
        IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisonContext comparisonContext);
    }
}