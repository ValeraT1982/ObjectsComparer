using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class ObjectsDataComparer<T> : IObjectDataComparer
    {
        public bool RecursiveComparison { get; set; }

        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();
        private readonly List<MemberInfo> _members;
        private IValueComparer _defaultValueComparer;

        public ObjectsDataComparer() : this(true) { }

        public ObjectsDataComparer(bool recursiveComparison)
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
            AddComparerOverride(typeof(string), new NulableStringsValueComparer());
        }


        public void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer)
        {
            _memberComparerOverrides[PropertyHelper.GetFieldInfo(memberLambda)] = memberValueComparer;
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
            if ((typeof(T)).IsComparable() || _typeComparerOverrides.ContainsKey(typeof(T)))
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
                    && type.InheritsFrom(typeof(ICollection<>)))
                {
                    var elementType = type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))
                        .Select(i => i.GetGenericArguments()[0])
                        .First();

                    var elementComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(elementType);
                    var elementComparer = (IObjectDataComparer)Activator.CreateInstance(elementComparerType);
                    ConfigureChildComparer(elementComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);
                    var collectionComparerType = typeof(CollectionsComparer<>).MakeGenericType(elementType);
                    var collectionComparer = (ICollectionsComparer)Activator.CreateInstance(collectionComparerType, elementComparer);

                    foreach (var difference in collectionComparer.Compare(value1, value2))
                    {
                        yield return difference.InsertPath(member.Name);
                    }
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && value1 != null
                    && value2 != null
                    && type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var elementComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(elementType);
                    var elementComparer = (IObjectDataComparer)Activator.CreateInstance(elementComparerType);
                    ConfigureChildComparer(elementComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);
                    var collectionType = typeof(Collection<>).MakeGenericType(elementType);
                    var expectedCollection = Activator.CreateInstance(collectionType, value1);
                    var actualCollection = Activator.CreateInstance(collectionType, value2);
                    var collectionComparerType = typeof(CollectionsComparer<>).MakeGenericType(elementType);
                    var collectionComparer = (ICollectionsComparer)Activator.CreateInstance(collectionComparerType, elementComparer);

                    foreach (var difference in collectionComparer.Compare(expectedCollection, actualCollection))
                    {
                        yield return difference.InsertPath(member.Name);
                    }

                    continue;
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && value1 != null
                    && value2 != null
                    && !type.IsComparable())
                {
                    var objectDataComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(type);
                    var objectDataComparer = (IObjectDataComparer)Activator.CreateInstance(objectDataComparerType);
                    ConfigureChildComparer(objectDataComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);

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

        private void ConfigureChildComparer(
            IObjectDataComparer comparer,
            IValueComparer defaultValueComparer,
            Dictionary<MemberInfo, IValueComparer> memberComparerOverrides,
            Dictionary<Type, IValueComparer> typeComparerOverrides)
        {
            comparer.SetDefaultComparer(defaultValueComparer);
            foreach (var memberComparerOverride in memberComparerOverrides)
            {
                comparer.AddComparerOverride(memberComparerOverride.Key, memberComparerOverride.Value);
            }

            foreach (var typeComparerOverride in typeComparerOverrides)
            {
                comparer.AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
            }
        }
    }
}
