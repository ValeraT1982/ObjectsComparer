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
        /// <param name="comparisonListener">
        /// Comparison process listener. If the argument is null the process is looking for all the differences at once.<br/>
        /// First parameter: Current comparison context, see <see cref="ComparisonContext"/>.<br/>
        /// Second parameter type: Whether to look for another difference. If value = false the comparison process will be terminated immediately.
        /// </param>
        /// <param name="differenceTreeCompleted">Occurs if (and only if) the comparison process reaches the last member of the objects being compared.</param>
        /// <returns>The root node of the difference tree, see <see cref="IDifferenceTreeNode"/>.</returns>
        public static IDifferenceTreeNode CalculateDifferenceTree(this IComparer comparer, Type type, object obj1, object obj2, Func<ComparisonContext, bool> comparisonListener = null, Action differenceTreeCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            comparisonListener = comparisonListener ?? ((_) => true);

            //Anything but ImplicitDifferenceTreeNode.
            var rootNode = DifferenceTreeNodeProvider.CreateNode(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(type, obj1, obj2, rootNode);

            differenceLocationList.EnumerateConditional(
                currentLocation =>
                {
                    return comparisonListener(new ComparisonContext(rootNode, currentLocation.Difference, currentLocation.TreeNode));
                },
                differenceTreeCompleted);

            return rootNode;
        }

        /// <summary>
        /// Calculates the difference tree.
        /// </summary>
        /// <param name="comparisonListener">
        /// Comparison process listener. If the argument is null the process is looking for all the differences at once.<br/>
        /// First parameter: Current comparison context, see <see cref="ComparisonContext"/>.<br/>
        /// Second parameter type: Whether to look for another difference. If value = false the comparison process will be terminated immediately.
        /// </param>
        /// <param name="differenceTreeCompleted">Occurs if (and only if) the comparison process reaches the last member of the objects being compared.</param>
        /// <returns>The root node of the difference tree, see <see cref="IDifferenceTreeNode"/>.</returns>
        public static IDifferenceTreeNode CalculateDifferenceTree<T>(this IComparer<T> comparer, T obj1, T obj2, Func<ComparisonContext, bool> comparisonListener = null, Action differenceTreeCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            comparisonListener = comparisonListener ?? ((_) => true);

            //Anything but ImplicitDifferenceTreeNode.
            var rootNode = DifferenceTreeNodeProvider.CreateNode(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(obj1, obj2, rootNode);

            differenceLocationList.EnumerateConditional(
                currentLocation =>
                {
                    return comparisonListener(new ComparisonContext(rootNode, currentLocation.Difference, currentLocation.TreeNode));
                },
                differenceTreeCompleted);

            return rootNode;
        }
    }
}

