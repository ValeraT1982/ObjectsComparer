using ObjectsComparer.Exceptions;
using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Extends interface <see cref="IComparer"/>.
    /// </summary>
    public static class IComparerExtensions
    {
        /// <summary>
        /// Calculates list of differences between objects. Accepts comparison context.
        /// </summary>
        /// <exception cref="ContextableComparerNotImplementedException">If <paramref name="comparer"/> does not implement <see cref="IContextableComparer"/>.</exception>
        public static IEnumerable<Difference> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, IComparisonContext comparisonContext)
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

            if (HasComparisonContextImplicitRoot(comparisonContext))
            {
                return comparer.CalculateDifferences(type, obj1, obj2);
            }

            //The caller passed on the root context, but did not provide an contextable comparer. The component guarantees that all its own comparers are contextable.
            throw new ContextableComparerNotImplementedException(comparer);
        }

        public static IEnumerable<Difference> CalculateDifferences<T>(this IComparer<T> comparer, T obj1, T obj2, IComparisonContext comparisonContext)
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

            if (HasComparisonContextImplicitRoot(comparisonContext))
            {
                return comparer.CalculateDifferences(obj1, obj2);
            }

            //The caller passed on the root context, but did not provide an contextable comparer. The component guarantees that all its own comparers are contextable.
            throw new ContextableComparerNotImplementedException(comparer);
        }

        static bool HasComparisonContextImplicitRoot(IComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            do
            {
                if (comparisonContext.Ancestor == null && comparisonContext is NullComparisonContext) 
                {
                    return true;
                }

                comparisonContext = comparisonContext.Ancestor;

            } while (comparisonContext != null);

            return false;
        }
    }
}