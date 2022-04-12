using ObjectsComparer.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;
using ObjectsComparer.Exceptions;

namespace ObjectsComparer.DifferenceTreeExtensions
{
    public static class ContextableExtensions
    {
        /// <summary>
        /// Builds the difference tree.
        /// </summary>
        /// <remarks>
        /// If <paramref name="comparer"/> is <see cref="IDifferenceTreeBuilder"/>, it looks for the difference, adds it to the difference tree and returns it, including its location.
        /// If not, it only looks for the difference and returns it with empty location. <br/> 
        /// It throws exception <see cref="DifferenceTreeBuilderNotImplementedException"/> if necessary. <br/>
        /// Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree(IComparer, Type, object, object, Func{DifferenceLocation, bool}, Action)"/> extension method instead.
        /// </remarks>
        /// <returns>The difference with its eventual location in the difference tree.</returns>
        public static IEnumerable<DifferenceLocation> TryBuildDifferenceTree(this IComparer comparer, Type type, object obj1, object obj2, IDifferenceTreeNode currentNode)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (currentNode is null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (comparer is IDifferenceTreeBuilder differenceTreeBuilder)
            {
                var differenceNodeLocationList = differenceTreeBuilder.BuildDifferenceTree(type, obj1, obj2, currentNode);

                foreach (var differenceNodeLocation in differenceNodeLocationList)
                {
                    yield return differenceNodeLocation;
                }

                yield break;
            }

            ThrowDifferenceTreeBuilderNotImplemented(currentNode, comparer.Settings, comparer, nameof(IDifferenceTreeBuilder));

            var differences = comparer.CalculateDifferences(type, obj1, obj2);

            foreach (var difference in differences)
            {
                yield return new DifferenceLocation(difference);
            }
        }

        /// <summary>
        /// Builds the difference tree. <br/>
        /// </summary>
        /// <remarks>
        /// If <paramref name="comparer"/> is <see cref="IDifferenceTreeBuilder"/>, it looks for the difference, adds it to the difference tree and returns it, including its location.
        /// If not, it only looks for the difference and returns it with empty location. <br/>
        /// It throws exception <see cref="DifferenceTreeBuilderNotImplementedException"/> if necessary.<br/>
        /// Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree{T}(IComparer{T}, T, T, Func{DifferenceLocation, bool}, Action)"/> extension method instead.
        /// </remarks>
        /// <returns>The difference with its eventual location in the difference tree.</returns>
        public static IEnumerable<DifferenceLocation> TryBuildDifferenceTree<T>(this IComparer<T> comparer, T obj1, T obj2, IDifferenceTreeNode comparisonContext)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (comparer is IDifferenceTreeBuilder<T> contextableComparer)
            {
                var differenceTreeNodeInfoList = contextableComparer.BuildDifferenceTree(obj1, obj2, comparisonContext);

                foreach (var differenceTreeNodeInfo in differenceTreeNodeInfoList)
                {
                    yield return differenceTreeNodeInfo;
                }

                yield break;
            }

            ThrowDifferenceTreeBuilderNotImplemented(comparisonContext, comparer.Settings, comparer, $"{nameof(IDifferenceTreeBuilder)}<{typeof(T).FullName}>");

            var differences = comparer.CalculateDifferences(obj1, obj2);

            foreach (var difference in differences)
            {
                yield return new DifferenceLocation(difference);
            }
        }

        static bool HasComparisonContextImplicitRoot(IDifferenceTreeNode comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            do
            {
                if (comparisonContext.Ancestor == null && comparisonContext is ImplicitDifferenceTreeNode)
                {
                    return true;
                }

                comparisonContext = comparisonContext.Ancestor;

            } while (comparisonContext != null);

            return false;
        }

        internal static void ThrowDifferenceTreeBuilderNotImplemented(IDifferenceTreeNode comparisonContext, ComparisonSettings comparisonSettings, object comparer, string unImplementedInterface)
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
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }

            if (comparisonSettings.ListComparisonOptionsAction != null)
            {
                var message = $"Because the list comparison was explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }

            //TODO: Check DifferenceOptionsAction

            if (HasComparisonContextImplicitRoot(comparisonContext) == false)
            {
                var message = $"Because the comparison context was explicitly passed, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }
        }
    }
}