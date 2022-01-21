using System;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class MultidimensionalArrayComparer<T> : AbstractComparer, IContextableComparer, IContextableComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public MultidimensionalArrayComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
            _comparer = Factory.GetObjectsComparer<T>(Settings, this);
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, new ComparisonContext());
        }

        public IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, IComparisonContext comparisonContext)
        {
            return CalculateDifferences(typeof(T), obj1, obj2, comparisonContext);
        }

        public IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, IComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (!type.InheritsFrom(typeof(Array)))
            {
                throw new ArgumentException("Invalid type");
            }

            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield break;
            }

            if (obj1 != null && !obj1.GetType().InheritsFrom(typeof(Array)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (obj2 != null && !obj2.GetType().InheritsFrom(typeof(Array)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var array1 = (Array)obj1 ?? Array.CreateInstance(typeof(T), new int[type.GetArrayRank()]);
            var array2 = (Array)obj2 ?? Array.CreateInstance(typeof(T), new int[type.GetArrayRank()]);

            if (array1.Rank != array2.Rank)
            {
                var difference = AddDifferenceToComparisonContext(new Difference("Rank", array1.Rank.ToString(), array2.Rank.ToString()), comparisonContext);
                yield return difference;
                yield break;
            }

            var dimensionsFailure = false;
            for (var i = 0; i < array1.Rank; i++)
            {
                var length1 = array1.GetLength(i);
                var length2 = array2.GetLength(i);

                // ReSharper disable once InvertIf
                if (length1 != length2)
                {
                    dimensionsFailure = true;
                    var difference = AddDifferenceToComparisonContext(new Difference($"Dimension{i}", length1.ToString(), length2.ToString()), comparisonContext);
                    yield return difference;
                }
            }

            if (dimensionsFailure)
            {
                yield break;
            }

            for (var i = 0; i < array1.Length; i++)
            {
                var indecies = IndexToCoordinates(array1, i);

                foreach (var failure in _comparer.CalculateDifferences((T)array1.GetValue(indecies), (T)array2.GetValue(indecies), comparisonContext))
                {
                    yield return failure.InsertPath($"[{string.Join(",", indecies)}]");
                }
            }
        }

        private static int[] IndexToCoordinates(Array arr, int i)
        {
            var dims = Enumerable.Range(0, arr.Rank)
                .Select(arr.GetLength)
                .ToArray();

            return dims.Select((d, n) => i / dims.Take(n).Aggregate(1, (i1, i2) => i1 * i2) % d).ToArray();
        }
    }
}
