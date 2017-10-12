using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> which provides implementation of Compare methods.
    /// </summary>
    public abstract class AbstractComparer<T>: BaseComparer, IComparer<T>
    {
        protected AbstractComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(obj1, obj2);

            return !differences.Any();
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare(T obj1, T obj2)
        {
            return !CalculateDifferences(obj1, obj2).Any();
        }

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        public abstract IEnumerable<Difference> CalculateDifferences(T obj1, T obj2);
    }
}