using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class EnumerablesComparer<T> : AbstractComparer
    {
        private readonly Comparer<T> _comparer;

        public EnumerablesComparer(ComparisonSettings settings, IBaseComparer parentComparer, IComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
            _comparer = new Comparer<T>(Settings, this);
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            if (!type.InheritsFrom(typeof(IEnumerable<>)))
            {
                throw new ArgumentException("Invalid type");
            }

            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
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

            var list1 = ((IEnumerable<T>)obj1).ToList();
            var list2 = ((IEnumerable<T>)obj2).ToList();

            if (list1.Count != list2.Count)
            {
                if (!type.InheritsFrom(typeof(ICollection<>)) && !type.GetTypeInfo().IsArray)
                {
                    yield return new Difference("Count", list1.Count.ToString(), list2.Count.ToString());
                }

                yield break;
            }

            for (int i = 0; i < list2.Count; i++)
            {
                foreach (var failure in _comparer.CalculateDifferences(list1[i], list2[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
