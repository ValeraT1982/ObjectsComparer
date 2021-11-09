using ObjectsComparer.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ObjectsComparer
{
    internal abstract class EnumerablesComparerBase : AbstractComparer
    {
        public EnumerablesComparerBase(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        /// <summary>
        /// Selects calculation operation based on the current value of the <see cref="ListConfigurationOptions.ElementSearchMode"/> property.
        /// </summary>
        protected virtual IEnumerable<Difference> CalculateDifferences<T>(IList<T> list1, IList<T> list2, ComparisonContext listComparisonContext, ListConfigurationOptions listConfigurationOptions)
        {
            if (listConfigurationOptions.ElementSearchMode == ListElementSearchMode.Key)
            {
                return CalculateDifferencesByKey(list1, list2, listComparisonContext, listConfigurationOptions);
            }
            else if (listConfigurationOptions.ElementSearchMode == ListElementSearchMode.Index)
            {
                return CalculateDifferencesByIndex(list1, list2, listComparisonContext);
            }
            else
            {
                throw new NotImplementedException($"{listConfigurationOptions.ElementSearchMode} not implemented yet.");
            }
        }

        /// <summary>
        /// Calculates differences using <see cref="ListElementSearchMode.Key"/> comparison mode.
        /// </summary>
        protected virtual IEnumerable<Difference> CalculateDifferencesByKey<T>(IList<T> array1, IList<T> array2, ComparisonContext listComparisonContext, ListConfigurationOptions listConfigurationOptions)
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(CalculateDifferencesByKey)}: {array1?.GetType().Name}");

            var keyOptions = CompareListElementsByKeyOptions.Default();
            listConfigurationOptions.KeyOptionsAction?.Invoke(keyOptions);

            for (int element1Index = 0; element1Index < array1.Count(); element1Index++)
            {
                var element1 = array1[element1Index];
                var elementComparisonContext = ComparisonContext.Create(ancestor: listComparisonContext);

                if (element1 == null)
                {
                    if (array2.Any(elm2 => elm2 == null))
                    {
                        continue;
                    }

                    var nullElementIdentifier = keyOptions.GetNullElementIdentifier(element1Index);

                    yield return AddDifferenceToComparisonContext(new Difference($"[{nullElementIdentifier}]", string.Empty, string.Empty, DifferenceTypes.MissedElementInSecondObject), elementComparisonContext);
                    continue;
                }

                var element1Key = keyOptions.KeyProviderAction(new ListElementKeyProviderArgs(element1));

                if (element1Key == null)
                {
                    if (keyOptions.ThrowKeyNotFoundEnabled)
                    {
                        throw new ElementKeyNotFoundException(element1, elementComparisonContext);
                    }

                    continue;
                }

                var formattedElement1Key = keyOptions.GetFormattedElementKey(new FormatListElementKeyArgs(element1Index, element1Key, element1));

                if (array2.Any(elm2 => object.Equals(element1Key, keyOptions.KeyProviderAction(new ListElementKeyProviderArgs(elm2)))))
                {
                    var element2 = array2.First(elm2 => object.Equals(element1Key, keyOptions.KeyProviderAction(new ListElementKeyProviderArgs(elm2))));
                    var comparer = Factory.GetObjectsComparer(element1.GetType(), Settings, this);

                    foreach (var failure in comparer.CalculateDifferences(element1.GetType(), element1, element2, elementComparisonContext))
                    {
                        yield return failure.InsertPath($"[{formattedElement1Key}]");
                    }
                }
                else
                {
                    var valueComparer1 = OverridesCollection.GetComparer(element1.GetType()) ?? DefaultValueComparer;
                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedElement1Key}]", valueComparer1.ToString(element1), string.Empty, DifferenceTypes.MissedElementInSecondObject), elementComparisonContext);
                }
            }

            for (int element2Index = 0; element2Index < array2.Count(); element2Index++)
            {
                var element2 = array2[element2Index];
                var elementComparisonContext = ComparisonContext.Create(ancestor: listComparisonContext);

                if (element2 == null)
                {
                    if (array1.Any(elm1 => elm1 == null))
                    {
                        continue;
                    }

                    var nullElementIdentifier = keyOptions.GetNullElementIdentifier(element2Index);

                    yield return AddDifferenceToComparisonContext(new Difference($"[{nullElementIdentifier}]", string.Empty, string.Empty, DifferenceTypes.MissedElementInFirstObject), elementComparisonContext);
                    continue;
                }

                var element2Key = keyOptions.KeyProviderAction(new ListElementKeyProviderArgs(element2));

                if (element2Key == null)
                {
                    if (keyOptions.ThrowKeyNotFoundEnabled)
                    {
                        throw new ElementKeyNotFoundException(element2, elementComparisonContext);
                    }

                    continue;
                }

                if (array1.Any(elm1 => object.Equals(element2Key, keyOptions.KeyProviderAction(new ListElementKeyProviderArgs(elm1)))) == false)
                {
                    var formattedElement2Key = keyOptions.GetFormattedElementKey(new FormatListElementKeyArgs(element2Index, element2Key, element2));
                    var valueComparer2 = OverridesCollection.GetComparer(element2.GetType()) ?? DefaultValueComparer;
                    yield return AddDifferenceToComparisonContext(new Difference($"[{formattedElement2Key}]", string.Empty, valueComparer2.ToString(element2), DifferenceTypes.MissedElementInFirstObject), elementComparisonContext);
                }
            }
        }

        /// <summary>
        /// Calculates differences using <see cref="ListElementSearchMode.Index"/> comparison mode.
        /// </summary>
        protected virtual IEnumerable<Difference> CalculateDifferencesByIndex<T>(IList<T> array1, IList<T> array2, ComparisonContext listComparisonContext)
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(CalculateDifferencesByIndex)}: {array1?.GetType().Name}");

            int array1Count = array1.Count();
            int array2Count = array2.Count();
            int smallerCount = array1Count <= array2Count ? array1Count : array2Count;

            //ToDo Extract type
            for (var i = 0; i < smallerCount; i++)
            {
                var elementComparisonContext = ComparisonContext.Create(ancestor: listComparisonContext);

                if (array1[i] == null && array2[i] == null)
                {
                    continue;
                }

                var valueComparer1 = array1[i] != null ? OverridesCollection.GetComparer(array1[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;
                var valueComparer2 = array2[i] != null ? OverridesCollection.GetComparer(array2[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                if (array1[i] == null)
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", string.Empty, valueComparer2.ToString(array2[i])), elementComparisonContext);
                    continue;
                }

                if (array2[i] == null)
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", valueComparer1.ToString(array1[i]), string.Empty), elementComparisonContext);
                    continue;
                }

                if (array1[i].GetType() != array2[i].GetType())
                {
                    yield return AddDifferenceToComparisonContext(new Difference($"[{i}]", valueComparer1.ToString(array1[i]), valueComparer2.ToString(array2[i]), DifferenceTypes.TypeMismatch), elementComparisonContext);
                    continue;
                }

                var comparer = Factory.GetObjectsComparer(array1[i].GetType(), Settings, this);

                foreach (var failure in comparer.CalculateDifferences(array1[i].GetType(), array1[i], array2[i], elementComparisonContext))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }

            //Add a "missed element" difference for each element that is in array1 and that is not in array2 or vice versa.
            if (array1Count != array2Count)
            {
                var largerArray = array1Count > array2Count ? array1 : array2;

                for (int i = smallerCount; i < largerArray.Count(); i++)
                {
                    var valueComparer = largerArray[i] != null ? OverridesCollection.GetComparer(largerArray[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                    var difference = new Difference(
                        memberPath: $"[{i}]",
                        value1: array1Count > array2Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        value2: array2Count > array1Count ? valueComparer.ToString(largerArray[i]) : string.Empty,
                        differenceType: array1Count > array2Count ? DifferenceTypes.MissedElementInSecondObject : DifferenceTypes.MissedElementInFirstObject);

                    yield return AddDifferenceToComparisonContext(difference, ComparisonContext.Create(ancestor: listComparisonContext));
                }
            }
        }
    }
}
