using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class ObjectsesDataComparer<T> : IObjectsDataComparer
    {
        public bool RecursiveComparison { get; set; }

        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();
        private readonly List<MemberInfo> _members;
        private IValueComparer _defaultValueComparer;

        public ObjectsesDataComparer() : this(true) { }

        public ObjectsesDataComparer(bool recursiveComparison)
        {
            var properties = typeof(T).GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();

            var fields = typeof(T).GetFields().Where(f =>
                f.IsPublic && !f.IsStatic).ToList();

            _members = properties.Union(fields.Cast<MemberInfo>()).ToList();

            RecursiveComparison = recursiveComparison;
            _defaultValueComparer = new DefaultValueComparer();
            //AddComparerOverride(typeof(string), new NulableStringsValueComparer());
        }

        public static IObjectsDataComparer CreateComparer(Type type)
        {
            var elementComparerType = typeof(ObjectsesDataComparer<>).MakeGenericType(type);

            return (IObjectsDataComparer)Activator.CreateInstance(elementComparerType);
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

        public void SetDefaultComparer(IValueComparer valueComparer)
        {
            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            _defaultValueComparer = valueComparer;
        }

        public IEnumerable<Difference> Compare(object obj1, object obj2)
        {
            return Compare((T)obj1, (T)obj2);
        }

        public IEnumerable<Difference> Compare(T obj1, T obj2)
        {
            if (typeof(T).IsComparable() || _typeComparerOverrides.ContainsKey(typeof(T)))
            {
                var comparer = _defaultValueComparer;
                if (_typeComparerOverrides.ContainsKey(typeof(T)))
                {
                    comparer = _typeComparerOverrides[typeof(T)];
                }

                if (!comparer.Compare(obj1, obj2))
                {
                    yield return new Difference(string.Empty, comparer.ToString(obj1), comparer.ToString(obj2));
                }
            }

            foreach (var member in _members)
            {
                var value1 = member.GetMemberValue(obj1);
                var value2 = member.GetMemberValue(obj2);
                var type = member.GetMemberType();

                var comparer = _defaultValueComparer;
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

                if (RecursiveComparison
                    && !hasCustomComparer
                    && value1 != null
                    && value2 != null
                    && !type.IsComparable()
                    && type.InheritsFrom(typeof(IEnumerable<>)))
                {
                    var elementType = type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(i => i.GetGenericArguments()[0])
                        .First();

                    var elementComparer = CreateComparer(elementType);
                    ConfigureChildComparer(elementComparer);
                    var enumerablesComparerType = typeof(EnumerablesComparer<>).MakeGenericType(elementType);
                    var enumerablesComparer = (IEnumerablesComparer)Activator.CreateInstance(enumerablesComparerType, elementComparer);

                    foreach (var difference in enumerablesComparer.Compare(value1, value2))
                    {
                        yield return difference.InsertPath(member.Name);
                    }
                }
                else if (RecursiveComparison
                    && !hasCustomComparer
                    && value1 != null
                    && value2 != null
                    && !type.IsComparable()
                    && type.InheritsFrom(typeof(IEnumerable)))
                {
                    var enumerablesComparer = new EnumerablesComparer(this);
                    foreach (var difference in enumerablesComparer.Compare(value1, value2))
                    {
                        yield return difference.InsertPath(member.Name);
                    }
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && value1 != null
                    && value2 != null
                    && !type.IsComparable())
                {
                    var objectDataComparer = CreateComparer(type);
                    ConfigureChildComparer(objectDataComparer);

                    objectDataComparer.SetDefaultComparer(_defaultValueComparer);
                    foreach (var typeComparerOverride in _typeComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
                    }

                    foreach (var memberComparerOverride in _memberComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(memberComparerOverride.Key, memberComparerOverride.Value);
                    }

                    foreach (var failure in objectDataComparer.Compare(value1, value2))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (!RecursiveComparison &&
                    !type.IsComparable())
                {
                    continue;
                }

                if (!comparer.Compare(value1, value2))
                {
                    yield return new Difference(member.Name, comparer.ToString(value1), comparer.ToString(value2));
                }
            }
        }

        public void ConfigureChildComparer(IObjectsDataComparer comparer)
        {
            comparer.SetDefaultComparer(_defaultValueComparer);
            foreach (var memberComparerOverride in _memberComparerOverrides)
            {
                comparer.AddComparerOverride(memberComparerOverride.Key, memberComparerOverride.Value);
            }

            foreach (var typeComparerOverride in _typeComparerOverrides)
            {
                comparer.AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
            }
        }
    }
}
