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
    internal class EnumerablesComparer : EnumerablesComparerBase, IComparerWithCondition, IContextableComparer
    {
        public EnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, ComparisonContext.CreateRoot());
        }

        public IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisonContext listComparisonContext)
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(CalculateDifferences)}: {type.Name}");

            if (listComparisonContext is null)
            {
                throw new ArgumentNullException(nameof(listComparisonContext));
            }

            if (!Settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield return AddDifferenceToComparisonContext(new Difference("[]", obj1?.ToString() ?? string.Empty, obj2?.ToString() ?? string.Empty), listComparisonContext);
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
            Settings.List.ConfigureOptionsAction?.Invoke(listComparisonContext, listConfigurationOptions);

            if (array1.Length != array2.Length)
            {
                yield return AddDifferenceToComparisonContext(new Difference("", array1.Length.ToString(), array2.Length.ToString(), DifferenceTypes.NumberOfElementsMismatch), listComparisonContext);

                if (listConfigurationOptions.UnequalListsComparisonEnabled == false)
                {
                    yield break;
                }
            }

            IEnumerable<Difference> failrues = CalculateDifferences(array1, array2, listComparisonContext, listConfigurationOptions);
            
            foreach (var failrue in failrues)
            {
                yield return failrue;
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
