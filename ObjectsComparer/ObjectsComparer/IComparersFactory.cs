using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Defines methods to create type specific comparers.
    /// </summary>
    public interface IComparersFactory
    {
        /// <summary>
        /// Creates type specific comparer.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="settings">Comparison Settings. Null by default.</param>
        /// <param name="parentComparer">Parent comparer. Null by default.</param>
        /// <returns>Instance of <see cref="IComparer{T}"/>.</returns>
        IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, BaseComparer parentComparer = null);

        /// <summary>
        /// Creates type specific comparer.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="settings">Comparison Settings. Null by default.</param>
        /// <param name="parentComparer">Parent comparer. Null by default.</param>
        /// <returns>Instance of <see cref="IComparer"/>.</returns>
        IComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, BaseComparer parentComparer = null);
    }
}