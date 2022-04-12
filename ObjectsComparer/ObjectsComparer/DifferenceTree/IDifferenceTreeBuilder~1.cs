using System;
using System.Collections.Generic;

namespace ObjectsComparer.DifferenceTreeExtensions
{
    /// <summary>
    /// Builds the difference tree.
    /// </summary>
    public interface IDifferenceTreeBuilder<T>
    {
        /// <summary>
        /// Finds the difference, adds it to the difference tree and returns it, including its location.
        /// </summary>
        /// <remarks>Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree{T}(IComparer{T}, T, T, Func{DifferenceLocation, bool}, Action)"/> extension method instead.</remarks>
        /// <returns>The location of the difference in the difference tree.</returns>
        IEnumerable<DifferenceLocation> BuildDifferenceTree(T obj1, T obj2, IDifferenceTreeNode comparisonContext);
    }
}
