using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.ContextExtensions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal abstract class AbstractDynamicObjectsComprer<T>: AbstractComparer, IComparerWithCondition, IDifferenceTreeBuilder, IDifferenceTreeBuilder<T>
    {
        protected AbstractDynamicObjectsComprer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            return AsContextableComparer().CalculateDifferences(type, obj1, obj2, ComparisonContextProvider.CreateImplicitRootContext(Settings))
                .Select(differenceLocation => differenceLocation.Difference);
        }

        IEnumerable<DifferenceLocation> IDifferenceTreeBuilder<T>.BuildDifferencesTree(T obj1, T obj2, IDifferenceTreeNode comparisonContext)
        {
            return AsContextableComparer().CalculateDifferences(typeof(T), obj1, obj2, ComparisonContextProvider.CreateImplicitRootContext(Settings));
        }

        IDifferenceTreeBuilder AsContextableComparer()
        {
            return this;
        }

        IEnumerable<DifferenceLocation> IDifferenceTreeBuilder.CalculateDifferences(Type type, object obj1, object obj2, IDifferenceTreeNode comparisonContext)
        {
            var castedObject1 = (T)obj1;
            var castedObject2 = (T)obj2;
            var propertyKeys1 = GetProperties(castedObject1);
            var propertyKeys2 = GetProperties(castedObject2);

            var propertyKeys = propertyKeys1.Union(propertyKeys2);

            foreach (var propertyKey in propertyKeys)
            {
                var existsInObject1 = propertyKeys1.Contains(propertyKey);
                var existsInObject2 = propertyKeys2.Contains(propertyKey);
                object value1 = null;
                MemberInfo member1 = null;
                if (existsInObject1)
                {
                    TryGetMemberValue(castedObject1, propertyKey, out value1);
                    TryGetMember(castedObject1, propertyKey, out member1);
                }

                object value2 = null;
                MemberInfo member2 = null;
                if (existsInObject2)
                {
                    TryGetMemberValue(castedObject2, propertyKey, out value2);
                    TryGetMember(castedObject1, propertyKey, out member2);
                }

                var keyComparisonContext = ComparisonContextProvider.CreateContext(Settings, comparisonContext, member1 ?? member2, propertyKey);

                var propertyType = (value1 ?? value2)?.GetType() ?? typeof(object);
                var customComparer = OverridesCollection.GetComparer(propertyType) ??
                                     OverridesCollection.GetComparer(propertyKey);
                var valueComparer = customComparer ?? DefaultValueComparer;

                if (Settings.UseDefaultIfMemberNotExist)
                {
                    if (!existsInObject1)
                    {
                        value1 = propertyType.GetDefaultValue();
                    }

                    if (!existsInObject2)
                    {
                        value2 = propertyType.GetDefaultValue();
                    }
                }

                if (!Settings.UseDefaultIfMemberNotExist)
                {
                    if (!existsInObject1)
                    {
                        var difference = AddDifferenceToTree(
                            new Difference(propertyKey, string.Empty, valueComparer.ToString(value2), DifferenceTypes.MissedMemberInFirstObject),
                            keyComparisonContext);

                        yield return difference;
                        continue;
                    }

                    if (!existsInObject2)
                    {
                        var difference = AddDifferenceToTree(
                            new Difference(propertyKey, valueComparer.ToString(value1), string.Empty, DifferenceTypes.MissedMemberInSecondObject),
                            keyComparisonContext);

                        yield return difference;
                        continue;
                    }
                }

                if (value1 != null && value2 != null && value1.GetType() != value2.GetType())
                {
                    var valueComparer2 = OverridesCollection.GetComparer(value2.GetType()) ??
                        OverridesCollection.GetComparer(propertyKey) ?? 
                        DefaultValueComparer;

                    var difference = AddDifferenceToTree(
                        new Difference(propertyKey, valueComparer.ToString(value1), valueComparer2.ToString(value2), DifferenceTypes.TypeMismatch),
                        keyComparisonContext);

                    yield return difference;
                    continue;
                }

                //null cannot be casted to ValueType
                if (value1 == null && value2 != null && value2.GetType().GetTypeInfo().IsValueType ||
                    value2 == null && value1 != null && value1.GetType().GetTypeInfo().IsValueType)
                {
                    var valueComparer2 = value2 != null ? 
                        OverridesCollection.GetComparer(value2.GetType()) ?? OverridesCollection.GetComparer(propertyKey) ?? DefaultValueComparer :
                        DefaultValueComparer;

                    var difference = AddDifferenceToTree(
                        new Difference(propertyKey, valueComparer.ToString(value1), valueComparer2.ToString(value2), DifferenceTypes.TypeMismatch),
                        keyComparisonContext);

                    yield return difference;
                    continue;
                }

                if (customComparer != null)
                {
                    if (!customComparer.Compare(value1, value2, Settings))
                    {
                        var differenceLocation = AddDifferenceToTree(
                            new Difference(propertyKey, customComparer.ToString(value1), customComparer.ToString(value2)),
                            keyComparisonContext);

                        yield return differenceLocation;
                    }

                    continue;
                }

                var comparer = Factory.GetObjectsComparer(propertyType, Settings, this);
                foreach (var failure in comparer.CalculateDifferences(propertyType, value1, value2, comparisonContext))
                {
                    failure.Difference.InsertPath(propertyKey);
                    yield return failure;
                }
            }
        }
        
        public abstract bool IsMatch(Type type, object obj1, object obj2);

        public abstract bool IsStopComparison(Type type, object obj1, object obj2);

        public abstract bool SkipMember(Type type, MemberInfo member);

        protected abstract IList<string> GetProperties(T obj);
        
        protected abstract bool TryGetMemberValue(T obj, string propertyName, out object value);

        protected abstract bool TryGetMember(T obj, string propertyName, out MemberInfo value);
    }
}