using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ObjectsComparer
{
    internal class CompilerGeneratedObjectComparer : AbstractComparer, IComparerWithCondition
    {
        public CompilerGeneratedObjectComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
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

            var propertyKeys = obj1.GetType().GetTypeInfo().GetMembers()
                .Union(obj1.GetType().GetTypeInfo().GetMembers())
                .Where(memberInfo => memberInfo is PropertyInfo)
                .Select(memberInfo => memberInfo.Name)
                .Distinct()
                .ToList();
            
            foreach (var propertyKey in propertyKeys)
            {
                var propertyInfo1 = obj1.GetType().GetTypeInfo().GetProperty(propertyKey);
                var propertyInfo2 = obj2.GetType().GetTypeInfo().GetProperty(propertyKey);
                var existsInObject1 = propertyInfo1 != null;
                var existsInObject2 = propertyInfo2 != null;
                var value1 = existsInObject1 ? propertyInfo1.GetValue(obj1) : null;
                var value2 = existsInObject2 ? propertyInfo2.GetValue(obj2) : null;
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

        public bool IsMatch(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
        }

        public bool IsStopComparison(Type type, object obj1, object obj2)
        {
            return true;
        }

        public bool SkipMember(Type type, MemberInfo member)
        {
            return false;
        }
    }
}
