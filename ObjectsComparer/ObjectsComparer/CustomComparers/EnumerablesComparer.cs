using System;
using System.Collections;
using System.Collections.Generic;
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
            return CalculateDifferences(type, obj1, obj2, ComparisonContext.Undefined);
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

            var listConfigurationOptions = ListConfigurationOptions.Default;
            Settings.List.ConfigureOptions?.Invoke(comparisonContext, listConfigurationOptions);

            if (array1.Length != array2.Length)
            {
                yield return new Difference("", array1.Length.ToString(), array2.Length.ToString(),
                    DifferenceTypes.NumberOfElementsMismatch);

                if (listConfigurationOptions.CompareUnequalLists == false)
                {
                    yield break;
                }
            }

            var smallerArray = array1.Length <= array2.Length ? array1 : array2;
            var largerArray = smallerArray == array1 ? array2 : array1;
            IEnumerable<Difference> failrues;

            if (listConfigurationOptions.KeyProvider != null)
            {
                failrues = CalculateDifferencesByKey(smallerArray, largerArray, comparisonContext, listConfigurationOptions);
            }
            else
            {
                failrues = CalculateDifferencesByIndex(smallerArray, largerArray, comparisonContext);
            }

            foreach (var failrue in failrues)
            {
                yield return failrue;
            }
        }

        private IEnumerable<Difference> CalculateDifferencesByKey(object[] smallerArray, object[] largerArray, ComparisonContext comparisonContext, ListConfigurationOptions listConfigurationOptions)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Difference> CalculateDifferencesByIndex(object[] smallerArray, object[] largerArray, ComparisonContext comparisonContext)
        {
            //ToDo Extract type
            for (var i = 0; i < smallerArray.Length; i++)
            {
                //List item has not got its MemberInfo, but has got its ancestor - list.
                var context = ComparisonContext.Create(currentMember: null, ancestor: comparisonContext);

                if (smallerArray[i] == null && largerArray[i] == null)
                {
                    continue;
                }

                var valueComparer1 = smallerArray[i] != null ? OverridesCollection.GetComparer(smallerArray[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;
                var valueComparer2 = largerArray[i] != null ? OverridesCollection.GetComparer(largerArray[i].GetType()) ?? DefaultValueComparer : DefaultValueComparer;

                if (smallerArray[i] == null)
                {
                    yield return new Difference($"[{i}]", string.Empty, valueComparer2.ToString(largerArray[i]));
                    continue;
                }

                if (largerArray[i] == null)
                {
                    yield return new Difference($"[{i}]", valueComparer1.ToString(smallerArray[i]), string.Empty);
                    continue;
                }

                if (smallerArray[i].GetType() != largerArray[i].GetType())
                {
                    yield return new Difference($"[{i}]", valueComparer1.ToString(smallerArray[i]), valueComparer2.ToString(largerArray[i]), DifferenceTypes.TypeMismatch);
                    continue;
                }

                var comparer = Factory.GetObjectsComparer(smallerArray[i].GetType(), Settings, this);
                foreach (var failure in comparer.CalculateDifferences(smallerArray[i].GetType(), smallerArray[i], largerArray[i], comparisonContext))
                {
                    yield return failure.InsertPath($"[{i}]");
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
