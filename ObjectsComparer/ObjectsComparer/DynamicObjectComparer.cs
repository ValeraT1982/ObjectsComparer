using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class DynamicObjectComparer : AbstractComparer, IComparerWithCondition
    {
        public DynamicObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                if (obj1 != obj2)
                {
                    yield return new Difference("", obj1?.ToString() ?? string.Empty,
                        obj2?.ToString() ?? string.Empty);
                    yield break;
                }

                yield break;
            }

            if (!type.InheritsFrom(typeof(DynamicObject)))
            {
                throw new ArgumentException(nameof(type));
            }

            if (!obj1.GetType().InheritsFrom(typeof(DynamicObject)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(DynamicObject)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var dynamicObject1 = (DynamicObject)obj1;
            var dynamicObject2 = (DynamicObject)obj2;
            var propertyKeys1 = dynamicObject1.GetDynamicMemberNames().ToList();
            var propertyKeys2 = dynamicObject2.GetDynamicMemberNames().ToList();

            var propertyKeys = propertyKeys1.Union(propertyKeys2);
            
            foreach (var propertyKey in propertyKeys)
            {
                var existsInObject1 = propertyKeys1.Contains(propertyKey);
                var existsInObject2 = propertyKeys2.Contains(propertyKey);
                var value1 = existsInObject1 ? dynamicObject1.GetType().GetTypeInfo().GetProperty(propertyKey)?.GetValue(dynamicObject1, null) : null;
                var value2 = existsInObject2 ? dynamicObject2.GetType().GetTypeInfo().GetProperty(propertyKey)?.GetValue(dynamicObject2, null) : null;
                var propertType = (value1 ?? value2)?.GetType() ?? typeof(object);
                var customComparer = OverridesCollection.GetComparer(propertType) ??
                                    OverridesCollection.GetComparer(propertyKey);
                var valueComparer = customComparer ?? DefaultValueComparer;

                if (!existsInObject1)
                {
                    yield return new Difference(propertyKey, string.Empty, valueComparer.ToString(value2),
                        DifferenceTypes.MissedMemberInFirstObject);
                    continue;
                }

                if (!existsInObject2)
                {
                    yield return new Difference(propertyKey, valueComparer.ToString(value1), string.Empty,
                        DifferenceTypes.MissedMemberInSecondObject);
                    continue;
                }

                if (value1 != null && value2 != null && value1.GetType() != value2.GetType())
                {
                    //It is OK because ToString conversion will be retired soon
                    yield return new Difference(propertyKey, value1.ToString(), value1.ToString(),
                        DifferenceTypes.TypeMismatch);
                    continue;
                }

                //null cannot be casted to ValueType
                if ((value1 == null && value2 != null && value2.GetType().GetTypeInfo().IsValueType) ||
                    (value2 == null && value1 != null && value1.GetType().GetTypeInfo().IsValueType))
                {
                    //It is OK because ToString conversion will be retired soon
                    yield return new Difference(propertyKey, value1?.ToString(), value2?.ToString(),
                        DifferenceTypes.TypeMismatch);
                    continue;
                }

                if (customComparer != null)
                {
                    if (!customComparer.Compare(value1, value2, Settings))
                    {
                        yield return new Difference(propertyKey, customComparer.ToString(value1), customComparer.ToString(value2));
                    }

                    continue;
                }

                var comparer = Factory.GetObjectsComparer(propertType, Settings, this);
                foreach (var failure in comparer.CalculateDifferences(propertType, value1, value2))
                {
                    yield return failure.InsertPath(propertyKey);
                }
            }
        }

        public bool IsMatch(Type type, object obj1, object obj2)
        {
            return type.InheritsFrom(typeof(DynamicObject));
        }

        public bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return false;
        }

        public bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }
    }
}
