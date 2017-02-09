using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public class EnumerablesComparer<T> : IEnumerablesComparer
    {
        private readonly IObjectsDataComparer _comparer;

        public EnumerablesComparer(IObjectsDataComparer elementComparer = null)
        {
            _comparer = elementComparer ?? new ObjectsesDataComparer<T>();
        }

        public IEnumerable<Difference> Compare(object obj1, object obj2)
        {
            obj1 = obj1 ?? Enumerable.Empty<T>();
            obj2 = obj2 ?? Enumerable.Empty<T>();

            if (obj1.GetType().IsAssignableFrom(typeof(IEnumerable<T>)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (obj2.GetType().IsAssignableFrom(typeof(IEnumerable<T>)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var expectedList = ((IEnumerable<T>)obj1).ToList();
            var actualList = ((IEnumerable<T>)obj2).ToList();

            if (expectedList.Count != actualList.Count)
            {
                yield return new Difference("[]", expectedList.Count.ToString(), actualList.Count.ToString());

                yield break;
            }

            for (int i = 0; i < actualList.Count; i++)
            {
                foreach (var failure in _comparer.Compare(expectedList[i], actualList[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
