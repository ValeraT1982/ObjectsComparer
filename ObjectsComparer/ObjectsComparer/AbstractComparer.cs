using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Implementation of <see cref="IComparer"/> which provides implementation of Compare methods.
    /// </summary>
    public abstract class AbstractComparer: BaseComparer, IComparer
    {
        protected AbstractComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) 
            : base(settings, parentComparer, factory)
        {
        }

        public abstract IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        public IEnumerable<Difference> CalculateDifferences<T>(T obj1, T obj2)
        {
            return CalculateDifferences(typeof(T), obj1, obj2);
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare(Type type, object obj1, object obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(type, obj1, obj2);

            return !differences.Any();
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare(Type type, object obj1, object obj2)
        {
            return !CalculateDifferences(type, obj1, obj2).Any();
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare<T>(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            return Compare(typeof(T), obj1, obj2, out differences);
        }

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        public bool Compare<T>(T obj1, T obj2)
        {
            return Compare(typeof(T), obj1, obj2);
        }
    }
}