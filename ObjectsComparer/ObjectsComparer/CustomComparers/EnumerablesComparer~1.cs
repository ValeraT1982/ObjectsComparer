using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ObjectsComparer.ContextExtensions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class EnumerablesComparer<T> : EnumerablesComparerBase, IContextableComparer, IContextableComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public EnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
            _comparer = Factory.GetObjectsComparer<T>(Settings, this);
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return CalculateDifferences(type, obj1, obj2, ComparisonContextProvider.CreateImplicitRootContext(Settings))
                .Select(differeneLocation => differeneLocation.Difference);
        }

        public IEnumerable<DifferenceLocation> CalculateDifferences(Type type, object obj1, object obj2, IComparisonContext listComparisonContext)
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(CalculateDifferences)}: {type.Name}");

            if (listComparisonContext is null)
            {
                throw new ArgumentNullException(nameof(listComparisonContext));
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

            var listComparisonOptions = ListComparisonOptions.Default();
            Settings.ListComparisonOptionsAction?.Invoke(listComparisonContext, listComparisonOptions);

            if (list1.Count != list2.Count)
            {
                if (!type.GetTypeInfo().IsArray)
                {
                    yield return AddDifferenceToTree(new Difference("", list1.Count().ToString(), list2.Count().ToString(), DifferenceTypes.NumberOfElementsMismatch), listComparisonContext);
                }

                if (listComparisonOptions.UnequalListsComparisonEnabled == false)
                {
                    yield break;
                }
            }

            var failrues = CalculateDifferences(list1, list2, listComparisonContext, listComparisonOptions);

            foreach (var failrue in failrues)
            {
                yield return failrue;
            }
        }

        public IEnumerable<DifferenceLocation> CalculateDifferences(T obj1, T obj2, IComparisonContext listComparisonContext)
        {
            return CalculateDifferences(((object)obj1 ?? obj2).GetType(), obj1, obj2, listComparisonContext);
        }
    }
}
