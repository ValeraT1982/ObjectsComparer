using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ObjectsComparer
{
    public class CollectionsComparer<T> : ICollectionsComparer
    {
        private readonly IObjectDataComparer _comparer;

        public CollectionsComparer(IObjectDataComparer elementComparer = null)
        {
            _comparer = elementComparer ?? new ObjectsDataComparer<T>();
        }

        public IEnumerable<Difference> Compare(object obj1, object obj2)
        {
            obj1 = obj1 ?? new Collection<T>();
            obj2 = obj2 ?? new Collection<T>();

            if (obj1.GetType().IsAssignableFrom(typeof(ICollection<T>)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (obj2.GetType().IsAssignableFrom(typeof(ICollection<T>)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var expectedList = ((ICollection<T>)obj1).ToList();
            var actualList = ((ICollection<T>)obj2).ToList();

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
