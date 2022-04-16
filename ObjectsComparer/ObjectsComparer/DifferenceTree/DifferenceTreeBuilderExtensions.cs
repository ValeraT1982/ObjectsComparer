using ObjectsComparer.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ObjectsComparer.DifferenceTreeExtensions
{
    public static class DifferenceTreeBuilderExtensions
    {
        /// <summary>
        /// If possible, creates a difference tree.
        /// </summary>
        /// <remarks>
        /// If <paramref name="comparer"/> is <see cref="IDifferenceTreeBuilder"/>, it builds the difference tree. If not, it only builds the flat list of differences.
        /// It throws <see cref="DifferenceTreeBuilderNotImplementedException"/> exception if necessary. <br/>
        /// Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree(IComparer, Type, object, object, Func{DifferenceLocation, bool}, Action)"/> extension method instead.
        /// </remarks>
        /// <returns>The differences with its eventual location in the difference tree.</returns>
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
        /// If possible, creates a difference tree.
        /// </summary>
        /// <remarks>
        /// If <paramref name="comparer"/> is <see cref="IDifferenceTreeBuilder"/>, it builds the difference tree. If not, it only builds the flat list of differences.
        /// It throws <see cref="DifferenceTreeBuilderNotImplementedException"/> exception if necessary. <br/>
        /// Intended for <see cref="IDifferenceTreeBuilder{T}"/> implementers. To avoid side effects, consumers should call <see cref="ComparerExtensions.CalculateDifferenceTree{T}(IComparer{T}, T, T, Func{DifferenceLocation, bool}, Action)"/> extension method instead.
        /// </remarks>
        /// <returns>The differences with its eventual location in the difference tree.</returns>
        public static IEnumerable<DifferenceLocation> TryBuildDifferenceTree<T>(this IComparer<T> comparer, T obj1, T obj2, IDifferenceTreeNode differenceTreeNode)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            if (comparer is IDifferenceTreeBuilder<T> differenceTreeBuilder)
            {
                var differenceTreeNodeInfoList = differenceTreeBuilder.BuildDifferenceTree(obj1, obj2, differenceTreeNode);

                foreach (var differenceTreeNodeInfo in differenceTreeNodeInfoList)
                {
                    yield return differenceTreeNodeInfo;
                }

                yield break;
            }

            ThrowDifferenceTreeBuilderNotImplemented(differenceTreeNode, comparer.Settings, comparer, $"{nameof(IDifferenceTreeBuilder)}<{typeof(T).FullName}>");

            var differences = comparer.CalculateDifferences(obj1, obj2);

            foreach (var difference in differences)
            {
                yield return new DifferenceLocation(difference);
            }
        }

        /// <summary>
        /// See <see cref="ImplicitDifferenceTreeNode"/>.
        /// </summary>
        static bool HasDifferenceTreeImplicitRoot(IDifferenceTreeNode differenceTreeNode)
        {
            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            do
            {
                if (differenceTreeNode.Ancestor == null && differenceTreeNode is ImplicitDifferenceTreeNode)
                {
                    return true;
                }

                differenceTreeNode = differenceTreeNode.Ancestor;

            } while (differenceTreeNode != null);

            return false;
        }

        internal static void ThrowDifferenceTreeBuilderNotImplemented(IDifferenceTreeNode differenceTreeNode, ComparisonSettings comparisonSettings, object comparer, string unImplementedInterface)
        {
            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            if (comparisonSettings is null)
            {
                throw new ArgumentNullException(nameof(comparisonSettings));
            }

            var options = DifferenceTreeOptions.Default();
            comparisonSettings.DifferenceTreeOptionsAction?.Invoke(null, options);

            if (options.ThrowDifferenceTreeBuilderNotImplementedEnabled == false)
            {
                return;
            }

            if (comparisonSettings.DifferenceTreeOptionsAction != null)
            {
                var message = $"Because the difference tree has been explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }

            if (comparisonSettings.ListComparisonOptionsAction != null)
            {
                var message = $"Because the list comparison has bben explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }

            //TODO: Check DifferenceOptionsAction

            if (HasDifferenceTreeImplicitRoot(differenceTreeNode) == false)
            {
                var message = $"Because the difference tree has been explicitly passed, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
                    "or throwing the DifferenceTreeBuilderNotImplementedException must be disabled.";
                throw new DifferenceTreeBuilderNotImplementedException(message);
            }
        }
    }
}