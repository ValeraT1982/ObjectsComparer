using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ObjectsComparer
{
    public class CollectionsComparer<T> : IComparer
    {
        private readonly IObjectDataComparer _comparer;

        public CollectionsComparer(IObjectDataComparer elementComparer = null)
        {
            _comparer = elementComparer ?? new ObjectsDataComparer<T>();
        }

        public IEnumerable<ComparisonFailure> Compare(object expected, object actual)
        {
            expected = expected ?? new Collection<T>();
            actual = actual ?? new Collection<T>();

            if (expected.GetType().IsAssignableFrom(typeof(ICollection<T>)))
            {
                throw new ArgumentException(nameof(expected));
            }

            if (actual.GetType().IsAssignableFrom(typeof(ICollection<T>)))
            {
                throw new ArgumentException(nameof(actual));
            }

            var expectedList = ((ICollection<T>)expected).ToList();
            var actualList = ((ICollection<T>)actual).ToList();

            if (expectedList.Count != actualList.Count)
            {
                yield return new ComparisonFailure("[]", expectedList.Count.ToString(), actualList.Count.ToString());

                yield break;
            }

            for (int i = 0; i < actualList.Count - 1; i++)
            {
                foreach (var failure in _comparer.Compare(expectedList[i], actualList[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
