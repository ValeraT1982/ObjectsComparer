using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class ExpandoObjectComparer : AbstractComparer, IComparerWithCondition
    {
        public ExpandoObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                if (obj1 != obj2)
                {
                    yield return new Difference("[]", obj1?.ToString() ?? string.Empty,
                        obj2?.ToString() ?? string.Empty);
                    yield break;
                }

                yield break;
            }

            if (!type.InheritsFrom(typeof(ExpandoObject)))
            {
                throw new ArgumentException(nameof(type));
            }

            if (!obj1.GetType().InheritsFrom(typeof(ExpandoObject)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(ExpandoObject)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var dictionary1 = (IDictionary<string, object>)obj1;
            var dictionary2 = (IDictionary<string, object>)obj2;

            var propertyKeys = dictionary1.Keys.Union(dictionary2.Keys);
            
            foreach (var propertyKey in propertyKeys)
            {
                var existsInObject1 = dictionary1.ContainsKey(propertyKey);
                var existsInObject2 = dictionary2.ContainsKey(propertyKey);
                var value1 = existsInObject1 ? dictionary1[propertyKey] : null;
                var value2 = existsInObject2 ? dictionary2[propertyKey] : null;
                var propertType = (value1 ?? value2)?.GetType() ?? typeof(object);
                var comparer = Factory.GetObjectsComparer(type, Settings, this);
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
                    yield return new Difference(propertyKey, string.Empty, string.Empty,
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

                foreach (var failure in comparer.CalculateDifferences(propertType, value1, value2))
                {
                    yield return failure.InsertPath(propertyKey);
                }
            }
        }

        public bool IsMatch(Type type)
        {
            return type.InheritsFrom(typeof(ExpandoObject));
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
