using ObjectsComparer.DifferenceTreeExtensions;
using ObjectsComparer.Exceptions;
using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public static class ComparerExtensions
    {
        /// <summary>
        /// XXX
        /// </summary>
        public static IDifferenceTreeNode CalculateDifferenceTree(this IComparer comparer, Type type, object obj1, object obj2, Func<DifferenceLocation, bool> findNextDifference = null, Action contextCompleted = null)
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

            //Anything but ImplicitComparisonContext (ImplicitDifferenceTreeNode).
            var rootNode = ComparisonContextProvider.CreateContext(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(type, obj1, obj2, rootNode);

            differenceLocationList.EnumerateConditional(
                currentLocation => 
                {
                    return findNextDifference(currentLocation);
                }, 
                contextCompleted);

            return rootNode;
        }

        /// <summary>
        /// XXX
        /// </summary>
        public static IDifferenceTreeNode CalculateDifferenceTree<T>(this IComparer<T> comparer, T obj1, T obj2, Func<DifferenceLocation, bool> findNextDifference = null, Action contextCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            findNextDifference = findNextDifference ?? ((_) => true);

            var rootCtx = ComparisonContextProvider.CreateContext(comparer.Settings, ancestor: null);

            var differenceLocationList = comparer.TryBuildDifferenceTree(obj1, obj2, rootCtx);

            differenceLocationList.EnumerateConditional(
                currentLocation =>
                {
                    return findNextDifference(currentLocation);
                },
                contextCompleted);

            return rootCtx;
        }

        internal static void EnumerateConditional<T>(this IEnumerable<T> enumerable, Func<T, bool> findNextElement = null, Action enumerationCompleted = null)
        {
            _ = enumerable ?? throw new ArgumentNullException(nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            var enumerationTerminated = false;

            while (enumerator.MoveNext())
            {
                if (findNextElement?.Invoke(enumerator.Current) == false)
                {
                    enumerationTerminated = true;
                    break;
                }
            }

            if (enumerationTerminated == false)
            {
                enumerationCompleted?.Invoke();
            }
        }
    }
}