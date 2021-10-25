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
                yield return AddDifferenceToComparisonContext(new Difference("[]", obj1?.ToString() ?? string.Empty, obj2?.ToString() ?? string.Empty), comparisonContext);
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
                yield return AddDifferenceToComparisonContext(new Difference("", array1.Length.ToString(), array2.Length.ToString(), DifferenceTypes.NumberOfElementsMismatch), comparisonContext);

                if (listConfigurationOptions.CompareUnequalLists == false)
                {
                    yield break;
                }
            }

            IEnumerable<Difference> failrues;

            if (listConfigurationOptions.ElementSearchMode == ListElementSearchMode.Key)
            {
                failrues = CalculateDifferencesByKey(array1, array2, comparisonContext, listConfigurationOptions);
            }
            else if (listConfigurationOptions.ElementSearchMode == ListElementSearchMode.Index) 
            {
                failrues = CalculateDifferencesByIndex(array1, array2, comparisonContext);
            }
            else
            {
                throw new NotImplementedException($"{listConfigurationOptions.ElementSearchMode} implemented yet.");
            }

            foreach (var failrue in failrues)
            {
                yield return failrue;
            }
        }

        private IEnumerable<Difference> CalculateDifferencesByKey(object[] array1, object[] array2, ComparisonContext listComparisonContext, ListConfigurationOptions listConfigurationOptions)
        {
            Debug.WriteLine(nameof(CalculateDifferencesByKey));

            var keyOptions = CompareElementsByKeyOptions.Default();
            listConfigurationOptions.KeyOptionsAction?.Invoke(keyOptions);

            for (int element1Index = 0; element1Index < array1.Length; element1Index++) 
            {
                var element1 = array1[element1Index];
                var elementContext = ComparisonContext.Create(member: null, ancestor: listComparisonContext);

                if (element1 == null)
                {
                    if (array2.Any(elm2 => elm2 == null))
                    {
                        continue;
                    }

                    var formattedNullElementIdentifier = keyOptions.GetFormattedNullElementIdentifier(element1Index);

                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedNullElementIdentifier}]", string.Empty, string.Empty, DifferenceTypes.MissedElementInSecondObject), elementContext);
                    continue;
                }

                var element1Key = keyOptions.KeyProvider(element1);

                if (element1Key == null)
                {
                    if (keyOptions.ThrowKeyNotFound)
                    {
                        throw new ElementKeyNotFoundException(element1);
                    }

                    continue;
                }

                var formattedElement1Key = keyOptions.GetFormattedElementKey(element1Index, element1Key);

                if (array2.Any(elm2 => object.Equals(element1Key, keyOptions.KeyProvider(elm2)))) 
                {
                    var element2 = array2.First(elm2 => object.Equals(element1Key, keyOptions.KeyProvider(elm2)));
                    var comparer = Factory.GetObjectsComparer(element1.GetType(), Settings, this);

                    foreach (var failure in comparer.CalculateDifferences(element1.GetType(), element1, element2, elementContext))
                    {
                        yield return failure.InsertPath($"[{formattedElement1Key}]");
                    }
                }
                else
                {
                    var valueComparer1 = OverridesCollection.GetComparer(element1.GetType()) ?? DefaultValueComparer;                    
                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedElement1Key}]", valueComparer1.ToString(element1), string.Empty, DifferenceTypes.MissedElementInSecondObject), elementContext);
                }
            }

            for (int element2Index = 0; element2Index < array2.Length; element2Index++) 
            {
                var element2 = array2[element2Index];

                if (element2 == null)
                {
                    if (array1.Any(elm1 => elm1 == null))
                    {
                        continue;
                    }

                    var elementContext = ComparisonContext.Create(member: null, ancestor: listComparisonContext);
                    var formattedNullElementIdentifier = keyOptions.GetFormattedNullElementIdentifier(element2Index);

                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedNullElementIdentifier}]", string.Empty, string.Empty, DifferenceTypes.MissedElementInFirstObject), elementContext);
                    continue;
                }

                var element2Key = keyOptions.KeyProvider(element2);

                if (element2Key == null)
                {
                    if (keyOptions.ThrowKeyNotFound)
                    {
                        throw new ElementKeyNotFoundException(element2);
                    }

                    continue;
                }

                if (array1.Any(elm1 => object.Equals(element2Key, keyOptions.KeyProvider(elm1))) == false) 
                {
                    var elementContext = ComparisonContext.Create(member: null, ancestor: listComparisonContext);
                    var formattedElement2Key = keyOptions.GetFormattedElementKey(element2Index, element2Key);
                    var valueComparer2 = OverridesCollection.GetComparer(element2.GetType()) ?? DefaultValueComparer;
                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedElement2Key}]", string.Empty, valueComparer2.ToString(element2), DifferenceTypes.MissedElementInFirstObject), elementContext);
                }
            }
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
                var elementContext = ComparisonContext.Create(member: null, ancestor: listComparisonContext);

                if (array1[i] == null && array2[i] == null)
                {
                    continue;
                }

                var valueComparer1 = array1[i] != null ? OverridesCollection.GetComparer(array1[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;
                var valueComparer2 = array2[i] != null ? OverridesCollection.GetComparer(array2[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                if (array1[i] == null)
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", string.Empty, valueComparer2.ToString(array2[i])), elementContext);
                    continue;
                }

                if (array2[i] == null)
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", valueComparer1.ToString(array1[i]), string.Empty), elementContext);
                    continue;
                }

                if (array1[i].GetType() != array2[i].GetType())
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", valueComparer1.ToString(array1[i]), valueComparer2.ToString(array2[i]), DifferenceTypes.TypeMismatch), elementContext);
                    continue;
                }

                var comparer = Factory.GetObjectsComparer(array1[i].GetType(), Settings, this);

                foreach (var failure in comparer.CalculateDifferences(array1[i].GetType(), array1[i], array2[i], elementContext))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }

            //Add a "missed element" difference for each element that is in array1 and that is not in array2 or vice versa.
            if (array1Count != array2Count)
            {
                var largerArray = array1Count > array2Count ? array1 : array2;

                for (int i = smallerCount; i < largerArray.Length; i++)
                {
                    var valueComparer = largerArray[i] != null ? OverridesCollection.GetComparer(largerArray[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                    var difference = new Difference(
                        memberPath: $"[{i}]",
                        value1: array1Count > array2Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        value2: array2Count > array1Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        differenceType: array1Count > array2Count ? DifferenceTypes.MissedElementInSecondObject : DifferenceTypes.MissedElementInFirstObject);

                    yield return AddDifferenceToComparisonContext(difference, ComparisonContext.Create(member: null, ancestor: listComparisonContext));
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
