using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Compares objects of type <see cref="T"/>.
    /// </summary>
    public class Comparer<T> : AbstractComparer<T>, IContextableComparer<T>
    {
        private readonly List<MemberInfo> _members;
        private readonly List<IComparerWithCondition> _conditionalComparers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T}" /> class. 
        /// </summary>
        /// <param name="settings">Comparison Settings.</param>
        /// <param name="parentComparer">Parent Comparer. Is used to copy DefaultValueComparer and Overrides. Null by default.</param>
        /// <param name="factory">Factory to create comparers in case of some members of the objects will need it.</param>
        public Comparer(ComparisonSettings settings = null, BaseComparer parentComparer = null, IComparersFactory factory = null)
            : base(settings, parentComparer, factory)
        {
            var properties = GetProperties(typeof(T), new List<Type>());
            var fields = typeof(T).GetTypeInfo().GetFields().Where(f =>
                f.IsPublic && !f.IsStatic).ToList();
            _members = properties.Union(fields.Cast<MemberInfo>()).ToList();
            _conditionalComparers = new List<IComparerWithCondition>
            {
                new MultidimensionalArraysComparer(Settings, this, Factory),
                new ExpandoObjectComparer(Settings, this, Factory),
                new DynamicObjectComparer(Settings, this, Factory),
                new CompilerGeneratedObjectComparer(Settings, this, Factory),
                new HashSetsComparer(Settings, this, Factory),
                new GenericEnumerablesComparer(Settings, this, Factory),
                new EnumerablesComparer(Settings, this, Factory),
                new TypesComparer(Settings, this, Factory)
            };

            // Additional value comparers
            AddComparerOverride<StringBuilder>(new ToStringComparer<StringBuilder>());
            AddComparerOverride<Uri>(new UriComparer());
        }

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        public override IEnumerable<Difference> CalculateDifferences(T obj1, T obj2)
        {
            return CalculateDifferences(obj1, obj2, memberInfo: null);
        }

        public IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, IComparisonContext comparisonContext)
        {
            return CalculateDifferences(obj1, obj2, memberInfo: null, comparisonContext);
        }

        internal IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, MemberInfo memberInfo)
        {
            return CalculateDifferences(obj1, obj2, memberInfo, ComparisonContext.Undefined);
        }

        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2, MemberInfo memberInfo, IComparisonContext comparisonContext)
        {
            var comparer = memberInfo != null
                ? OverridesCollection.GetComparer(memberInfo)
                : OverridesCollection.GetComparer(typeof(T));

            if (typeof(T).IsComparable() ||
                comparer != null)
            {
                comparer = comparer ?? DefaultValueComparer;
                if (!comparer.Compare(obj1, obj2, Settings))
                {
                    yield return
                        new Difference(string.Empty, comparer.ToString(obj1),
                            comparer.ToString(obj2));
                }

                yield break;
            }

            var conditionalComparer = _conditionalComparers.FirstOrDefault(c => c.IsMatch(typeof(T), obj1, obj2));
            if (conditionalComparer != null)
            {
                foreach (var difference in conditionalComparer.CalculateDifferences(typeof(T), obj1, obj2, comparisonContext))
                {
                    yield return difference;
                }

                if (conditionalComparer.IsStopComparison(typeof(T), obj1, obj2))
                {
                    yield break;
                }
            }

            if (obj1 == null || obj2 == null)
            {
                if (!DefaultValueComparer.Compare(obj1, obj2, Settings))
                {
                    yield return new Difference(string.Empty, DefaultValueComparer.ToString(obj1), DefaultValueComparer.ToString(obj2));
                }

                yield break;
            }

            if (!Settings.RecursiveComparison)
            {
                yield break;
            }

            foreach (var member in _members)
            {                
                var value1 = member.GetMemberValue(obj1);
                var value2 = member.GetMemberValue(obj2);
                var type = member.GetMemberType();

                if (conditionalComparer != null && conditionalComparer.SkipMember(typeof(T), member))
                {
                    continue;
                }

                var context = ComparisonContext.Create(currentMember: member, ancestor: comparisonContext);

                var valueComparer = DefaultValueComparer;
                var hasCustomComparer = false;

                var comparerOverride = OverridesCollection.GetComparer(member);
                if (comparerOverride != null)
                {
                    valueComparer = comparerOverride;
                    hasCustomComparer = true;
                }

                if (!hasCustomComparer
                    && !type.IsComparable())
                {
                    var objectDataComparer = Factory.GetObjectsComparer(type, Settings, this);

                    foreach (var failure in objectDataComparer.CalculateDifferences(type, value1, value2, context))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (!valueComparer.Compare(value1, value2, Settings))
                {
                    yield return new Difference(member.Name, valueComparer.ToString(value1), valueComparer.ToString(value2));
                }
            }
        }

        private List<PropertyInfo> GetProperties(Type type, List<Type> processedTypes)
        {
            var properties = type.GetTypeInfo().GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();
            processedTypes.Add(type);

            if (type.GetTypeInfo().IsInterface)
            {
                foreach (var parrentInterface in type.GetTypeInfo().GetInterfaces())
                {
                    if (processedTypes.Contains(parrentInterface))
                    {
                        continue;
                    }

                    properties = properties
                        .Union(GetProperties(parrentInterface, processedTypes))
                        .Distinct()
                        .ToList();
                }
            }

            return properties;
        }

        
    }
}
