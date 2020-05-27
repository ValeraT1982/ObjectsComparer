using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer.CustomComparers
{
    public class ComparableEnumerablesComparer<T> : AbstractComparer
        where T : IComparableEnumerableItem
    {
        public ComparableEnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory) { }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            var group = typeof(T).GetGroupName(Settings);

            if (!type.InheritsFrom(typeof(IEnumerable<>)))
            {
                throw new ArgumentException("Invalid type");
            }

            if (!Settings.EmptyAndNullEnumerablesEqual
                && (obj1 == null || obj2 == null) && obj1 != obj2)
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

            var list1 = ((IEnumerable<T>)obj1).Select((o, i) => new { Index = i, Object = o });
            var list2 = ((IEnumerable<T>)obj2).Select((o, i) => new { Index = i, Object = o });

            var intersection = list1.Join(list2, o1 => o1.Object.Key, o2 => o2.Object.Key, (o1, o2) => new { O1 = o1, O2 = o2 });
            foreach (var item in intersection)
            {
                var comparer = Factory.GetObjectsComparer(item.O1.Object.GetType(), Settings, this);
                foreach (var failure in comparer.CalculateDifferences(item.O1.Object.GetType(), item.O1.Object, item.O2.Object))
                {
                    yield return failure.InsertPath($"[{item.O1.Index + 1}]");
                }
            }

            var addedItems = list2.Where(e => !list1.Any(o1 => e.Object.Key == o1.Object.Key));
            foreach (var item in addedItems)
            {
                yield return new Difference(group, $"[{item.Index + 1}]", string.Empty, "Added", DifferenceTypes.MissedElementInFirstObject);
            }

            var deletedItems = list1.Where(e => !list2.Any(o2 => e.Object.Key == o2.Object.Key));
            foreach (var item in deletedItems)
            {
                yield return new Difference(group, $"[{item.Index + 1}]", "Deleted", string.Empty, DifferenceTypes.MissedMemberInSecondObject);
            }
        }
    }
}
