using ObjectsComparer.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ObjectsComparer.ContextExtensions
{
    public static class ContextableExtensions
    {
        /// <summary>
        /// Calculates list of differences between objects. Accepts comparison context.
        /// </summary>
        /// <remarks>The method is intended for IContextableComparer implementers.</remarks>
        public static IEnumerable<DifferenceTreeNodeInfo> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, IComparisonContext comparisonContext)
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
                var differenceTreeNodeInfoList = contextableComparer.CalculateDifferences(type, obj1, obj2, comparisonContext);

                foreach (var differenceTreeNodeInfo in differenceTreeNodeInfoList)
                {
                    yield return differenceTreeNodeInfo;
                }
            }

            ThrowContextableComparerNotImplemented(comparisonContext, comparer.Settings, comparer, nameof(IContextableComparer));

            var differences = comparer.CalculateDifferences(type, obj1, obj2);

            foreach (var difference in differences)
            {
                yield return new DifferenceTreeNodeInfo(difference);
            }

        }

        /// <summary>
        /// Calculates list of differences between objects. Accepts comparison context.
        /// </summary>
        /// <remarks>The method is intended for IContextableComparer implementers.</remarks>
        public static IEnumerable<DifferenceTreeNodeInfo> CalculateDifferences<T>(this IComparer<T> comparer, T obj1, T obj2, IComparisonContext comparisonContext)
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
                var differenceTreeNodeInfoList = contextableComparer.CalculateDifferences(obj1, obj2, comparisonContext);

                foreach (var differenceTreeNodeInfo in differenceTreeNodeInfoList)
                {
                    yield return differenceTreeNodeInfo;
                }
            }

            ThrowContextableComparerNotImplemented(comparisonContext, comparer.Settings, comparer, $"{nameof(IContextableComparer)}<{typeof(T).FullName}>");

            var differences = comparer.CalculateDifferences(obj1, obj2);

            foreach (var difference in differences)
            {
                yield return new DifferenceTreeNodeInfo(difference);
            }
        }

        static bool HasComparisonContextImplicitRoot(IComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            do
            {
                if (comparisonContext.Ancestor == null && comparisonContext is ImplicitComparisonContext)
                {
                    return true;
                }

                comparisonContext = comparisonContext.Ancestor;

            } while (comparisonContext != null);

            return false;
        }

        static void ThrowContextableComparerNotImplemented(IComparisonContext comparisonContext, ComparisonSettings comparisonSettings, object comparer, string unImplementedInterface)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (comparisonSettings is null)
            {
                throw new ArgumentNullException(nameof(comparisonSettings));
            }

            var options = ComparisonContextOptions.Default();
            comparisonSettings.ComparisonContextOptionsAction?.Invoke(null, options);

            if (options.ThrowContextableComparerNotImplementedEnabled == false)
            {
                return;
            }

            if (comparisonSettings.ComparisonContextOptionsAction != null)
            {
                var message = $"Because the comparison context was explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the ContextableComparerNotImplementedException must be disabled.";
                throw new ContextableComparerNotImplementedException(message);
            }

            if (comparisonSettings.ListComparisonOptionsAction != null)
            {
                var message = $"Because the list comparison was explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the ContextableComparerNotImplementedException must be disabled.";
                throw new ContextableComparerNotImplementedException(message);
            }

            //TODO: Check DifferenceOptionsAction

            if (HasComparisonContextImplicitRoot(comparisonContext) == false)
            {
                var message = $"Because the comparison context was explicitly passed, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the ContextableComparerNotImplementedException must be disabled.";
                throw new ContextableComparerNotImplementedException(message);
            }
        }
    }
}