using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Implements Comparers Factory.
    /// </summary>
    public class ComparersFactory : IComparersFactory
    {
        /// <summary>
        /// Creates type specific comparer.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="settings">Comparison Settings. Null by default.</param>
        /// <param name="parentComparer">Parent comparer. Null by default.</param>
        /// <returns>Instance of <see cref="IComparer{T}"/>.</returns>
        public virtual IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, BaseComparer parentComparer = null)
        {
            return new Comparer<T>(settings, parentComparer);
        }

        /// <summary>
        /// Creates type specific comparer.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="settings">Comparison Settings. Null by default.</param>
        /// <param name="parentComparer">Parent comparer. Null by default.</param>
        /// <returns>Instance of <see cref="IComparer"/>.</returns>
        public IComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, BaseComparer parentComparer = null)
        {
            return new Comparer(settings, parentComparer, this);
        }
    }
}
