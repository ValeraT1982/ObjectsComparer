using ObjectsComparer.ContextExtensions;
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
        ///// <summary>
        ///// Calculates list of differences between objects. Accepts comparison context.
        ///// </summary>
        //public static IEnumerable<Difference> CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, IComparisonContext comparisonContext)
        //{
        //    if (comparer is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparer));
        //    }

        //    if (type is null)
        //    {
        //        throw new ArgumentNullException(nameof(type));
        //    }

        //    if (comparisonContext is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparisonContext));
        //    }

        //    if (comparer is IContextableComparer contextableComparer)
        //    {
        //        return contextableComparer.CalculateDifferences(type, obj1, obj2, comparisonContext);
        //    }

        //    ThrowContextableComparerNotImplemented(comparisonContext, comparer.Settings, comparer, nameof(IContextableComparer));

        //    return comparer.CalculateDifferences(obj1, obj2);
        //}

        ///// <summary>
        ///// Calculates list of differences between objects. Accepts comparison context.
        ///// </summary>
        //public static IEnumerable<Difference> CalculateDifferences<T>(this IComparer<T> comparer, T obj1, T obj2, IComparisonContext comparisonContext)
        //{
        //    if (comparer is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparer));
        //    }

        //    if (comparisonContext is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparisonContext));
        //    }

        //    if (comparer is IContextableComparer<T> contextableComparer)
        //    {
        //        return contextableComparer.CalculateDifferences(obj1, obj2, comparisonContext);
        //    }

        //    ThrowContextableComparerNotImplemented(comparisonContext, comparer.Settings, comparer, $"{nameof(IContextableComparer)}<{typeof(T).FullName}>");

        //    return comparer.CalculateDifferences(obj1, obj2);
        //}

        //static bool HasComparisonContextImplicitRoot(IComparisonContext comparisonContext)
        //{
        //    if (comparisonContext is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparisonContext));
        //    }

        //    do
        //    {
        //        if (comparisonContext.Ancestor == null && comparisonContext is ImplicitComparisonContext) 
        //        {
        //            return true;
        //        }

        //        comparisonContext = comparisonContext.Ancestor;

        //    } while (comparisonContext != null);

        //    return false;
        //}

        //static void ThrowContextableComparerNotImplemented(IComparisonContext comparisonContext, ComparisonSettings comparisonSettings, object comparer, string unImplementedInterface)
        //{
        //    if (comparisonContext is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparisonContext));
        //    }

        //    if (comparisonSettings is null)
        //    {
        //        throw new ArgumentNullException(nameof(comparisonSettings));
        //    }

        //    var options = ComparisonContextOptions.Default();
        //    comparisonSettings.ComparisonContextOptionsAction?.Invoke(null, options);

        //    if (options.ThrowContextableComparerNotImplementedEnabled == false)
        //    {
        //        return;
        //    }

        //    if (comparisonSettings.ComparisonContextOptionsAction != null)
        //    {
        //        var message = $"Because the comparison context was explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
        //            "or throwing the ContextableComparerNotImplementedException must be disabled.";
        //        throw new ContextableComparerNotImplementedException(message);
        //    }

        //    if (comparisonSettings.ListComparisonOptionsAction != null)
        //    {
        //        var message = $"Because the list comparison was explicitly configured, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
        //            "or throwing the ContextableComparerNotImplementedException must be disabled.";
        //        throw new ContextableComparerNotImplementedException(message);
        //    }

        //    //TODO: Check DifferenceOptionsAction

        //    if (HasComparisonContextImplicitRoot(comparisonContext) == false) 
        //    {
        //        var message = $"Because the comparison context was explicitly passed, the {comparer.GetType().FullName} must implement {unImplementedInterface} interface " +
        //            "or throwing the ContextableComparerNotImplementedException must be disabled.";
        //        throw new ContextableComparerNotImplementedException(message);
        //    }
        //}

        public static IComparisonContext CalculateDifferences(this IComparer comparer, Type type, object obj1, object obj2, Func<IComparisonContext, Difference, bool> findNextDifference = null, Action contextCompleted = null)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var rootCtx = ComparisonContextProvider.CreateContext(comparer.Settings, ancestor: null);
            var differences = comparer.CalculateDifferences(type, obj1, obj2, rootCtx);

            differences.EnumerateConditional(
                currentDifference => 
                {
                    return findNextDifference(null, currentDifference);
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