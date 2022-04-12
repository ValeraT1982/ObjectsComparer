using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.ContextExtensions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Compares objects.
    /// </summary>
    public class Comparer : AbstractComparer, IDifferenceTreeBuilder
    {
        private static string CalculateDifferencesMethodName
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            get { return MemberInfoExtensions.GetMethodName<Comparer<object>>(x => x.CalculateDifferences(null, null)); }
        }

        private static string BuildDifferenceTreeMethodName
        {
            get { return MemberInfoExtensions.GetMethodName<IDifferenceTreeBuilder<object>>(x => x.BuildDifferenceTree(null, null, null)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer" /> class. 
        /// </summary>
        /// <param name="settings">Comparison Settings.</param>
        /// <param name="parentComparer">Parent Comparer. Is used to copy DefaultValueComparer and Overrides. Null by default.</param>
        /// <param name="factory">Factory to create comparers in case of some members of the objects will need it.</param>
        public Comparer(ComparisonSettings settings = null, BaseComparer parentComparer = null, IComparersFactory factory = null) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return AsContextableComparer().BuildDifferenceTree(type, obj1, obj2, ComparisonContextProvider.CreateImplicitRootContext(Settings))
                .Select(differenceLoccation => differenceLoccation.Difference);
        }

        IDifferenceTreeBuilder AsContextableComparer() => this;

        IEnumerable<DifferenceLocation> IDifferenceTreeBuilder.BuildDifferenceTree(Type type, object obj1, object obj2, IDifferenceTreeNode comparisonContext)
        {
            if (comparisonContext is null)
            {
                throw new ArgumentNullException(nameof(comparisonContext));
            }

            var getObjectsComparerMethod = typeof(IComparersFactory).GetTypeInfo().GetMethods().First(m => m.IsGenericMethod);
            var getObjectsComparerGenericMethod = getObjectsComparerMethod.MakeGenericMethod(type);
            var comparer = getObjectsComparerGenericMethod.Invoke(Factory, new object[] { Settings, this });

            bool comparerIsIContextableComparerT = comparer.GetType().GetTypeInfo().GetInterfaces()
                .Any(intft => intft.GetTypeInfo().IsGenericType && intft.GetGenericTypeDefinition() == typeof(IDifferenceTreeBuilder<>));

            if (comparerIsIContextableComparerT == false)
            {
                if (comparer is IDifferenceTreeBuilder contextableComparer)
                {
                    var diffLocationList = contextableComparer.BuildDifferenceTree(type, obj1, obj2, comparisonContext);

                    foreach (var diffLocation in diffLocationList)
                    {
                        yield return diffLocation;
                    }

                    yield break;
                }

                ContextableExtensions.ThrowContextableComparerNotImplemented(comparisonContext, Settings, comparer, $"{nameof(IDifferenceTreeBuilder)}<{type.FullName}>");
            }

            var genericType = comparerIsIContextableComparerT ? typeof(IDifferenceTreeBuilder<>).MakeGenericType(type) : typeof(IComparer<>).MakeGenericType(type);
            var genericMethodName = comparerIsIContextableComparerT ? BuildDifferenceTreeMethodName : CalculateDifferencesMethodName;
            var genericMethodParameterTypes = comparerIsIContextableComparerT ? new[] { type, type, typeof(IDifferenceTreeNode) } : new[] { type, type };            
            var genericMethod = genericType.GetTypeInfo().GetMethod(genericMethodName, genericMethodParameterTypes);
            var genericMethodParameters = comparerIsIContextableComparerT ? new[] { obj1, obj2, comparisonContext } : new[] { obj1, obj2 };

            // ReSharper disable once PossibleNullReferenceException
            //return (IEnumerable<DifferenceLocation>)genericMethod.Invoke(comparer, genericMethodParameters);

            var returnValue = genericMethod.Invoke(comparer, genericMethodParameters);

            if (returnValue is IEnumerable<DifferenceLocation> differenceLocationList)
            {
                foreach (var differenceLocation in differenceLocationList)
                {
                    yield return differenceLocation;
                }

                yield break;
            }

            if (returnValue is IEnumerable<Difference> differenceList)
            {
                foreach (var difference in differenceList)
                {
                    yield return new DifferenceLocation(difference);
                }

                yield break;
            }

            //TODO: 
            throw new NotImplementedException("");
        }
    }
}
