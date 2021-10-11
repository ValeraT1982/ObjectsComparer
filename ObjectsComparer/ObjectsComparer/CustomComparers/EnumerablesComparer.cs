using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class EnumerablesComparer : AbstractComparer, IComparerWithCondition, IContextableComparer
    {
        public EnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, ComparisonContext.CreateRoot());
        }

        public IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisonContext comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield return new Difference("[]", obj1?.ToString() ?? string.Empty, obj2?.ToString() ?? string.Empty);
                yield break;
            }
            
            obj1 = obj1 ?? Enumerable.Empty<object>();
            obj2 = obj2 ?? Enumerable.Empty<object>();

            if (!type.InheritsFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(type));
            }

            if (!obj1.GetType().InheritsFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var array1 = ((IEnumerable)obj1).Cast<object>().ToArray();
            var array2 = ((IEnumerable)obj2).Cast<object>().ToArray();

            var listConfigurationOptions = ListConfigurationOptions.Default();
            Settings.List.ConfigureOptionsAction?.Invoke(comparisonContext, listConfigurationOptions);

            if (array1.Length != array2.Length)
            {
                yield return new Difference("", array1.Length.ToString(), array2.Length.ToString(),
                    DifferenceTypes.NumberOfElementsMismatch);

                if (listConfigurationOptions.CompareUnequalLists == false)
                {
                    yield break;
                }
            }

            IEnumerable<Difference> failrues;

            if (listConfigurationOptions.ComparisonMode == ListElementComparisonMode.Key)
            {
                failrues = CalculateDifferencesByKey(array1, array2, comparisonContext, listConfigurationOptions);
            }
            else if (listConfigurationOptions.ComparisonMode == ListElementComparisonMode.Index)
            {
                failrues = CalculateDifferencesByIndex(array1, array2, comparisonContext);
            }
            else
            {
                throw new NotImplementedException($"ListElementComparisonMode is not implemented {listConfigurationOptions.ComparisonMode}.");
            }

            foreach (var failrue in failrues)
            {
                yield return failrue;
            }
        }

        private IEnumerable<Difference> CalculateDifferencesByKey(object[] array1, object[] array2, ComparisonContext listComparisonContext, ListConfigurationOptions listConfigurationOptions)
        {
            var keyOptions = CompareElementsByKeyOptions.Default();
            listConfigurationOptions.KeyOptionsAction?.Invoke(keyOptions);

            foreach (var element1 in array1)
            {
                if (element1 == null)
                {
                    if (array2.Any(elm2 => elm2 == null))
                    {
                        continue;
                    }

                    yield return new Difference("[Key = NULL]", string.Empty, string.Empty, DifferenceTypes.MissedElementInSecondObject);
                    continue;
                }

                var element1Key = keyOptions.KeyProvider(element1);

                if (element1Key == null)
                {
                    if (keyOptions.ThrowKeyNotFound)
                    {
                        throw new ElementNotFoundByKeyException();
                    }
                    continue;
                }
            }

            Debug.WriteLine(nameof(CalculateDifferencesByKey));
        }

        private IEnumerable<Difference> CalculateDifferencesByIndex(object[] array1, object[] array2, ComparisonContext listComparisonContext)
        {
            Debug.WriteLine(nameof(CalculateDifferencesByIndex));

            int array1Count = array1.Count();
            int array2Count = array2.Count();
            int smallerCount = array1Count <= array2Count ? array1Count : array2Count;

            //ToDo Extract type
            for (var i = 0; i < smallerCount; i++)
            {
                //Context representing the element has no member. Its ancestor is the context representing the list.
                var elementComparisonContext = ComparisonContext.Create(currentMember: null, ancestor: listComparisonContext);

                if (array1[i] == null && array2[i] == null)
                {
                    continue;
                }

                var valueComparer1 = array1[i] != null ? OverridesCollection.GetComparer(array1[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;
                var valueComparer2 = array2[i] != null ? OverridesCollection.GetComparer(array2[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                if (array1[i] == null)
                {
                    yield return new Difference($"[{i}]", string.Empty, valueComparer2.ToString(array2[i]));
                    continue;
                }

                if (array2[i] == null)
                {
                    yield return new Difference($"[{i}]", valueComparer1.ToString(array1[i]), string.Empty);
                    continue;
                }

                if (array1[i].GetType() != array2[i].GetType())
                {
                    yield return new Difference($"[{i}]", valueComparer1.ToString(array1[i]), valueComparer2.ToString(array2[i]), DifferenceTypes.TypeMismatch);
                    continue;
                }

                var comparer = Factory.GetObjectsComparer(array1[i].GetType(), Settings, this);

                foreach (var failure in comparer.CalculateDifferences(array1[i].GetType(), array1[i], array2[i], elementComparisonContext))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }

            //Add a "missed element" difference for each element that is in array1 and is not in array2, or vice versa. The positions of value1 and value2 are respected in Difference instance.
            if (array1Count != array2Count)
            {
                var largerArray = array1Count > array2Count ? array1 : array2;

                for (int i = smallerCount; i < largerArray.Length; i++)
                {
                    var valueComparer = largerArray[i] != null ? OverridesCollection.GetComparer(largerArray[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                    yield return new Difference(
                        memberPath: $"[{i}]",
                        value1: array1Count > array2Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        value2: array2Count > array1Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        differenceType: array1Count > array2Count ? DifferenceTypes.MissedElementInSecondObject : DifferenceTypes.MissedElementInFirstObject);
                }
            }
        }

        public bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(IEnumerable)) && !type.InheritsFrom(typeof(IEnumerable<>));
        }

        public bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null;
        }

        public bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }
    }
}
