using System;
using System.Collections.Generic;

namespace ObjectsComparer.ContextExtensions
{
    /// <summary>
    /// Comparer accepting <see cref="IComparisonContext"/>.
    /// </summary>
    public interface IContextableComparer
    {
        IEnumerable<DifferenceTreeNodeInfo> CalculateDifferences(Type type, object obj1, object obj2, IComparisonContext comparisonContext);
    }
}