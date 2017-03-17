using System;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class EnumerablesComparer<T> : AbstractObjectsDataComparer<T>
    {
        private readonly IObjectsDataComparer _comparer;

        public EnumerablesComparer(ComparisonSettings settings, IObjectsDataComparer parentComparer, IObjectsComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
            _comparer = new ObjectsDataComparer<T>(Settings, this);
        }

        public override IEnumerable<Difference> CalculateDifferences(object obj1, object obj2)
        {
            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield return new Difference("[]", obj1?.ToString() ?? string.Empty, obj2?.ToString() ?? string.Empty);
                yield break;
            }

            obj1 = obj1 ?? Enumerable.Empty<T>();
            obj2 = obj2 ?? Enumerable.Empty<T>();

            if (!obj1.GetType().InheritsFrom(typeof(IEnumerable<T>)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(IEnumerable<T>)))
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
                foreach (var failure in _comparer.CalculateDifferences(expectedList[i], actualList[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
