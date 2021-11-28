using ObjectsComparer.Exceptions;
using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Extends interface <see cref="IComparer"/> with an overloaded operation CalculateDifferences, that accepts <see cref="ComparisonContext"/> parameter.
    /// </summary>
    public static class IComparerExtensions
    {
        /// <summary>
        /// Calculates list of differences between objects. Accepts comparison context.
        /// At the beginning of the comparison you can create <see cref="ComparisonContext"/> instance using the <see cref="ComparisonContext.CreateRoot"/> operation and pass it as a parameter.
        /// For more info about comparison context see <see cref="ComparisonContext"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="comparisonContext">Current comparison context. For more info see <see cref="ComparisonContext"/> class.</param>
        /// <returns>List of differences between objects.</returns>
        /// <exception cref="ContextableComparerNotImplementedException">If <paramref name="comparer"/> does not implement <see cref="IContextableComparer"/>.</exception>
        public static IEnumerable<Difference> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, ComparisonContext comparisonContext)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (comparer is IContextableComparer contextableComparer)
            {
                return contextableComparer.CalculateDifferences(type, obj1, obj2, comparisonContext);
            }

            throw new ContextableComparerNotImplementedException(comparer);
            //return comparer.CalculateDifferences(type, obj1, obj2);
        }

        public static IEnumerable<Difference> CalculateDifferences<T>(this IComparer<T> comparer, T obj1, T obj2, ComparisonContext comparisonContext)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (comparer is IContextableComparer<T> contextableComparer)
            {
                return contextableComparer.CalculateDifferences(obj1, obj2, comparisonContext);
            }

            throw new ContextableComparerNotImplementedException(comparer);
            //return comparer.CalculateDifferences(obj1, obj2);
        }
    }
}