using System;
using System.Collections.Generic;

namespace ObjectsComparer.ContextExtensions
{
    /// <summary>
    /// Builds the difference tree.
    /// </summary>
    public interface IDifferenceTreeBuilder<T>
    {
        /// <summary>
        /// Finds the difference, adds it to the difference tree and returns it, including its location.
        /// </summary>
        /// <remarks>Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferencesTree{T}(IComparer{T}, T, T, Func{DifferenceLocation, bool}, Action)"/> extension method instead.</remarks>
        /// <returns>The location of the difference in the difference tree.</returns>
        IEnumerable<DifferenceLocation> BuildDifferencesTree(T obj1, T obj2, IDifferenceTreeNode comparisonContext);
    }
}
