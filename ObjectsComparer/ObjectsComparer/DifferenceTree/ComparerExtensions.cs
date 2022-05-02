using ObjectsComparer.DifferenceTreeExtensions;
using ObjectsComparer.Exceptions;
using ObjectsComparer.Utils;
using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public static class ComparerExtensions
    {
        /// <summary>
        /// Calculates the difference tree.
        /// </summary>
        /// <param name="findNextDifference">Current comparison context, see <see cref="ComparisonContext"/>. The return value tells the comparison process whether to look for another difference. If the argument is null the process is looking for all the differences.</param>
        /// <param name="differenceTreeCompleted">Occurs when (and only when) the comparison process reaches the last member of the objects being compared.</param>
        /// <returns>The root node of the difference tree.</returns>
        public static IDifferenceTreeNode CalculateDifferenceTree(this IComparer comparer, Type type, object obj1, object obj2, Func<ComparisonContext, bool> findNextDifference = null, Action differenceTreeCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            findNextDifference = findNextDifference ?? ((_) => true);

            //Anything but ImplicitDifferenceTreeNode.
            var rootNode = DifferenceTreeNodeProvider.CreateNode(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(type, obj1, obj2, rootNode);

            differenceLocationList.EnumerateConditional(
                currentLocation =>
                {
                    return findNextDifference(new ComparisonContext(rootNode, currentLocation.Difference, currentLocation.TreeNode));
                },
                differenceTreeCompleted);

            return rootNode;
        }

        /// <summary>
        /// Calculates the difference tree.
        /// </summary>
        /// <param name="findNextDifference">Current comparison context. The return value tells the comparison process whether to look for another difference. If the argument is null the process is looking for all the differences.</param>
        /// <param name="differenceTreeCompleted">Occurs when (and only when) the comparison process reaches the last member of the objects being compared.</param>
        /// <returns>The root node of the difference tree.</returns>
        public static IDifferenceTreeNode CalculateDifferenceTree<T>(this IComparer<T> comparer, T obj1, T obj2, Func<ComparisonContext, bool> findNextDifference = null, Action differenceTreeCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            findNextDifference = findNextDifference ?? ((_) => true);

            //Anything but ImplicitDifferenceTreeNode.
            var rootNode = DifferenceTreeNodeProvider.CreateNode(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(obj1, obj2, rootNode);

            differenceLocationList.EnumerateConditional(
                currentLocation =>
                {
                    return findNextDifference(new ComparisonContext(rootNode, currentLocation.Difference, currentLocation.TreeNode));
                },
                differenceTreeCompleted);

            return rootNode;
        }
    }
}

