using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class EnumerablesComparer<T> : AbstractComparer, IContextableComparer, IContextableComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public EnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
            _comparer = Factory.GetObjectsComparer<T>(Settings, this);
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, ComparisonContext.Undefined);
        }

        public IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

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
                if (!type.GetTypeInfo().IsArray)
                {
                    yield return new Difference("", list1.Count.ToString(), list2.Count.ToString(),
                        DifferenceTypes.NumberOfElementsMismatch);
                }

                yield break;
            }

            for (var i = 0; i < list2.Count; i++)
            {
                //List item has not got its MemberInfo, but has got its ancestor - list.
                var context = ComparisonContext.Create(currentMember: null, ancestor: comparisonContext);

                foreach (var failure in _comparer.CalculateDifferences(list1[i], list2[i], context))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }

        public IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, ComparisonContext comparisonContext)
        {
            return CalculateDifferences(((object)obj1 ?? obj2).GetType(), obj1, obj2, comparisonContext);
        }
    }
}
