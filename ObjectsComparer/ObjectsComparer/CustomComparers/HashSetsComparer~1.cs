using System;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class HashSetsComparer<T> : AbstractComparer, IContextableComparer, IContextableComparer<T>
    {
        public HashSetsComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
        }

        public IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, ComparisonContext comparisonContext)
        {
            return CalculateDifferences(typeof(T), obj1, obj2, comparisonContext);
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, new ComparisonContext());
        }

        public IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (!type.InheritsFrom(typeof(HashSet<>)))
            {
                throw new ArgumentException("Invalid type");
            }

            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield break;
            }

            obj1 = obj1 ?? new HashSet<T>();
            obj2 = obj2 ?? new HashSet<T>();

            if (!obj1.GetType().InheritsFrom(typeof(HashSet<T>)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(HashSet<T>)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var hashSet1 = ((IEnumerable<T>)obj1).ToList();
            var hashSet2 = ((IEnumerable<T>)obj2).ToList();
            var valueComparer = OverridesCollection.GetComparer(typeof(T)) ?? DefaultValueComparer;

            foreach (var element in hashSet1)
            {
                if (!hashSet2.Contains(element))
                {
                    var difference = AddDifferenceToComparisonContext(
                        new Difference("", valueComparer.ToString(element), string.Empty, DifferenceTypes.MissedElementInSecondObject), 
                        comparisonContext);

                    yield return difference;
                }
            }
            
            foreach (var element in hashSet2)
            {
                if (!hashSet1.Contains(element))
                {
                    var difference = AddDifferenceToComparisonContext(new Difference("", string.Empty, valueComparer.ToString(element),
                        DifferenceTypes.MissedElementInFirstObject), comparisonContext);

                    yield return difference;
                }
            }
        }
              

        public bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(HashSet<>));
        }
    }
}
