using System;
using System.Collections.Generic;

namespace ObjectsComparer.Utils
{
    internal static class EnumerableExtensions
    {
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