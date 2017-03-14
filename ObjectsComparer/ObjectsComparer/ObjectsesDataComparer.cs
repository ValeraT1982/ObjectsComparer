using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class ObjectsDataComparer<T> : IObjectsDataComparer<T>
    {
        public ComparisonSettings Settings { get; }

        public IEnumerable<KeyValuePair<MemberInfo, IValueComparer>> MemberComparerOverrides => _memberComparerOverrides.Select(o => o);

        public IEnumerable<KeyValuePair<Type, IValueComparer>> TypeComparerOverrides => _typeComparerOverrides.Select(o => o);

        public IValueComparer DefaultValueComparer { get; private set; }

        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();
        private readonly List<MemberInfo> _members;
        
        public ObjectsDataComparer(IObjectsDataComparer parentComparer = null) : this(new ComparisonSettings(), parentComparer) { }

        public ObjectsDataComparer(ComparisonSettings settings, IObjectsDataComparer parentComparer = null)
        {
            Settings = settings ?? new ComparisonSettings();
            var properties = typeof(T).GetTypeInfo().GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();
            var fields = typeof(T).GetTypeInfo().GetFields().Where(f =>
                f.IsPublic && !f.IsStatic).ToList();
            _members = properties.Union(fields.Cast<MemberInfo>()).ToList();
            DefaultValueComparer = new DefaultValueComparer();
            if (parentComparer != null)
            {
                DefaultValueComparer = parentComparer.DefaultValueComparer;
                foreach (var memberComparerOverride in parentComparer.MemberComparerOverrides)
                {
                    AddComparerOverride(memberComparerOverride.Key, memberComparerOverride.Value);
                }

                foreach (var typeComparerOverride in parentComparer.TypeComparerOverrides)
                {
                    AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
                }
            }
        }

        public static IObjectsDataComparer CreateComparer(Type type, ComparisonSettings settings, IObjectsDataComparer parentComparer = null)
        {
            var elementComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(type);

            return (IObjectsDataComparer)Activator.CreateInstance(elementComparerType, settings, parentComparer);
        }

        public void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer)
        {
            _memberComparerOverrides[PropertyHelper.GetMemberInfo(memberLambda)] = memberValueComparer;
        }

        public void AddComparerOverride(MemberInfo memberInfo, IValueComparer memberValueComparer)
        {
            _memberComparerOverrides[memberInfo] = memberValueComparer;
        }

        public void AddComparerOverride(Type type, IValueComparer typeValueComparer)
        {
            _typeComparerOverrides[type] = typeValueComparer;
        }

        public void AddComparerOverride<TType>(IValueComparer typeValueComparer)
        {
            AddComparerOverride(typeof(TType), typeValueComparer);
        }

        public void SetDefaultComparer(IValueComparer valueComparer)
        {
            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            DefaultValueComparer = valueComparer;
        }

        public bool Compare(object obj1, object obj2, out IEnumerable<Difference> differences)
        {
            return Compare((T) obj1, (T) obj2, out differences);
        }

        public bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(obj1, obj2);

            return !differences.Any();
        }

        public bool Compare(object obj1, object obj2)
        {
            return !CalculateDifferences((T)obj1, (T)obj2).Any();
        }

        public bool Compare(T obj1, T obj2)
        {
            return !CalculateDifferences(obj1, obj2).Any();
        }

        public IEnumerable<Difference> CalculateDifferences(object obj1, object obj2)
        {
            if (typeof(T).IsComparable() ||
                _typeComparerOverrides.ContainsKey(typeof(T)))
            {
                var comparer = DefaultValueComparer;
                if (_typeComparerOverrides.ContainsKey(typeof(T)))
                {
                    comparer = _typeComparerOverrides[typeof(T)];
                }

                if (!comparer.Compare(obj1, obj2, Settings))
                {
                    yield return
                        new Difference(string.Empty, comparer.ToString(obj1),
                            comparer.ToString(obj2));
                }

                yield break;
            }

            if (!typeof(T).IsComparable() && typeof(T).InheritsFrom(typeof(IEnumerable<>)))
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
                
                var elementComparer = CreateComparer(elementType, Settings, this);
                var enumerablesComparerType = typeof(EnumerablesComparer<>).MakeGenericType(elementType);
                var enumerablesComparer = (IEnumerablesComparer)Activator.CreateInstance(enumerablesComparerType, Settings, elementComparer);

                foreach (var difference in enumerablesComparer.Compare(obj1, obj2))
                {
                    yield return difference;
                }

                if (Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null)
                {
                    yield break;
                }
            }
            else if (!typeof(T).IsComparable()
                     && typeof(T).InheritsFrom(typeof(IEnumerable)))
            {
                var enumerablesComparer = new EnumerablesComparer(Settings, this);
                foreach (var difference in enumerablesComparer.Compare(obj1, obj2))
                {
                    yield return difference;
                }

                if (Settings.EmptyAndNullEnumerablesEqual && obj1 == null || obj2 == null)
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

                var comparer = DefaultValueComparer;
                var hasCustomComparer = false;

                if (_memberComparerOverrides.ContainsKey(member))
                {
                    comparer = _memberComparerOverrides[member];
                    hasCustomComparer = true;
                }
                else if (_typeComparerOverrides.ContainsKey(type))
                {
                    comparer = _typeComparerOverrides[type];
                    hasCustomComparer = true;
                }

                if (!hasCustomComparer
                    && !type.IsComparable())
                {
                    var objectDataComparer = CreateComparer(type, Settings, this);

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
