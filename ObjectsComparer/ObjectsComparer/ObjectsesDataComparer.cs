using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class ObjectsDataComparer<T> : AbstractObjectsDataComparer<T>
    {
        private readonly List<MemberInfo> _members;
        private readonly List<IObjectsDataComparerWithCondition> _conditionalComparers;

        public ObjectsDataComparer(ComparisonSettings settings = null, IObjectsDataComparer parentComparer = null, IObjectsComparersFactory factory = null) 
            : base(settings, parentComparer, factory)
        {
            var properties = typeof(T).GetTypeInfo().GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();
            var fields = typeof(T).GetTypeInfo().GetFields().Where(f =>
                f.IsPublic && !f.IsStatic).ToList();
            _members = properties.Union(fields.Cast<MemberInfo>()).ToList();
            _conditionalComparers = new List<IObjectsDataComparerWithCondition>
            {
                new EnumerablesComparer(Settings, this, Factory)
            };
        }

        public override IEnumerable<Difference> CalculateDifferences(object obj1, object obj2)
        {
            if (typeof(T).IsComparable() ||
                TypeComparerOverrides.Any(p => p.Key == typeof(T)))
            {
                var comparer = DefaultValueComparer;
                if (TypeComparerOverrides.Any(p => p.Key == typeof(T)))
                {
                    comparer = TypeComparerOverrides.First(p => p.Key == typeof(T)).Value;
                }

                if (!comparer.Compare(obj1, obj2, Settings))
                {
                    yield return
                        new Difference(string.Empty, comparer.ToString(obj1),
                            comparer.ToString(obj2));
                }

                yield break;
            }

            var conditionalComparer = _conditionalComparers.FirstOrDefault(c => c.IsMatch(typeof(T)));

            if (conditionalComparer != null)
            {
                foreach (var difference in conditionalComparer.CalculateDifferences(obj1, obj2))
                {
                    yield return difference;
                }

                if (conditionalComparer.IsStopComparison(obj1, obj2))
                {
                    yield break;
                }
            }

            if (typeof(T).InheritsFrom(typeof(IEnumerable<>)))
            {
                Type elementType;

                if (typeof(T).GetTypeInfo().IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = typeof(T).GetTypeInfo().GetGenericArguments()[0];
                }
                else
                {
                    elementType = typeof(T).GetTypeInfo().GetInterfaces()
                        .Where(i => i.GetTypeInfo().IsGenericType && i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(i => i.GetTypeInfo().GetGenericArguments()[0])
                        .First();
                }

                var enumerablesComparerType = typeof(EnumerablesComparer<>).MakeGenericType(elementType);
                var enumerablesComparer = (IObjectsDataComparer)Activator.CreateInstance(enumerablesComparerType, Settings, this, Factory);

                foreach (var difference in enumerablesComparer.CalculateDifferences(obj1, obj2))
                {
                    yield return difference;
                }

                if (Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null)
                {
                    yield break;
                }
            }
            //else if (typeof(T).InheritsFrom(typeof(IEnumerable)))
            //{
            //    var enumerablesComparer = new EnumerablesComparer(Settings, this, Factory);
            //    foreach (var difference in enumerablesComparer.CalculateDifferences(obj1, obj2))
            //    {
            //        yield return difference;
            //    }

            //    if (Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null)
            //    {
            //        yield break;
            //    }
            //}

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

                var comparer = DefaultValueComparer;
                var hasCustomComparer = false;

                if (MemberComparerOverrides.Any(p => p.Key == member))
                {
                    comparer = MemberComparerOverrides.First(p => p.Key == member).Value;
                    hasCustomComparer = true;
                }
                else if (TypeComparerOverrides.Any(p => p.Key == type))
                {
                    comparer = TypeComparerOverrides.First(p => p.Key == type).Value;
                    hasCustomComparer = true;
                }

                if (!hasCustomComparer
                    && !type.IsComparable())
                {
                    var objectDataComparer = Factory.GetObjectsComparer(type, Settings, this);

                    foreach (var failure in objectDataComparer.CalculateDifferences(value1, value2))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (!comparer.Compare(value1, value2, Settings))
                {
                    yield return new Difference(member.Name, comparer.ToString(value1), comparer.ToString(value2));
                }
            }
        }
    }
}
