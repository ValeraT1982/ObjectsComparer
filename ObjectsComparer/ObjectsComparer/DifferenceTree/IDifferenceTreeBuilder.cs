using System;
using System.Collections.Generic;

namespace ObjectsComparer.DifferenceTreeExtensions
{
    /// <summary>
    /// Builds the difference tree.
    /// </summary>
    public interface IDifferenceTreeBuilder
    {
        /// <summary>
        /// Finds the difference, adds it to the difference tree and returns it, including its location.
        /// </summary>
        /// <remarks>Intended for <see cref="IDifferenceTreeBuilder"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree(IComparer, Type, object, object, Func{DifferenceLocation, bool}, Action)"/> extension method instead.</remarks>
        /// <returns>The location of the difference in the difference tree.</returns>
        IEnumerable<DifferenceLocation> BuildDifferenceTree(Type type, object obj1, object obj2, IDifferenceTreeNode comparisonContext);
    }
}