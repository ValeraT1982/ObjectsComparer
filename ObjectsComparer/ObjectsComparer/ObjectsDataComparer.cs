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
        public bool SkipDefaultValues { get; set; }
        public bool RecursiveComparison { get; set; }

        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();
        private readonly List<MemberInfo> _members;
        private IValueComparer _defaultValueComparer;

        public ObjectsDataComparer()
        {
            var properties = typeof(T).GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();

            var fields = typeof(T).GetFields().Where(f =>
                f.IsPublic && !f.IsStatic).ToList();

            _members = properties.Union(fields.Cast<MemberInfo>()).ToList();

            SkipDefaultValues = true;
            RecursiveComparison = true;
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

        public IEnumerable<ComparisonFailure> Compare(object expectedObject, object actualObject)
        {
            return Compare((T)expectedObject, (T)actualObject);
        }

        public IEnumerable<ComparisonFailure> Compare(T expectedObject, T actualObject)
        {
            foreach (var member in _members)
            {
                var expectedValue = member.GetMemberValue(expectedObject);
                var actualValue = member.GetMemberValue(actualObject);
                var type = member.GetMemberType();

                if (SkipDefaultValues && expectedValue.IsDefaultValue())
                {
                    continue;
                }

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
                    && expectedValue != null
                    && actualValue != null
                    && type.IsClass
                    && type != typeof(string)
                    && !type.IsAssignableFrom(typeof(IComparable))
                    && !type.IsAssignableFrom(typeof(IComparable<>))
                    && type.IsAssignableFrom(typeof(ICollection<>)))
                {
                    var elementType = type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))
                        .Select(i => i.GetGenericArguments()[0])
                        .First();

                    var elementComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(elementType);
                    var elementComparer = (IObjectDataComparer)Activator.CreateInstance(elementComparerType);
                    ConfigureChildComparer(elementComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);
                    var collectionComparerType = typeof(CollectionsComparer<>).MakeGenericType(elementType);
                    var collectionComparer = (IObjectDataComparer)Activator.CreateInstance(collectionComparerType, elementComparer);
                    
                    foreach (var failure in collectionComparer.Compare(expectedValue, actualValue))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && expectedValue != null
                    && actualValue != null
                    && type.IsClass
                    && type != typeof(string)
                    && type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var elementComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(elementType);
                    var elementComparer = (IObjectDataComparer)Activator.CreateInstance(elementComparerType);
                    ConfigureChildComparer(elementComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);
                    var collectionType = typeof(Collection<>).MakeGenericType(elementType);
                    var expectedCollection = Activator.CreateInstance(collectionType, expectedValue);
                    var actualCollection = Activator.CreateInstance(collectionType, actualValue);
                    var collectionComparerType = typeof(CollectionsComparer<>).MakeGenericType(elementType);
                    var collectionComparer = (IComparer)Activator.CreateInstance(collectionComparerType, elementComparer);

                    foreach (var failure in collectionComparer.Compare(expectedCollection, actualCollection))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && expectedValue != null
                    && actualValue != null
                    && type.IsClass
                    && type != typeof(string)
                    && !type.IsAssignableFrom(typeof(IComparable))
                    && !type.IsAssignableFrom(typeof(IComparable<>)))
                {
                    var objectDataComparerType = typeof(ObjectsDataComparer<>).MakeGenericType(type);
                    var objectDataComparer = (IObjectDataComparer)Activator.CreateInstance(objectDataComparerType);
                    ConfigureChildComparer(objectDataComparer, _defaultValueComparer, _memberComparerOverrides, _typeComparerOverrides);

                    objectDataComparer.SkipDefaultValues = SkipDefaultValues;
                    objectDataComparer.RecursiveComparison = true;
                    objectDataComparer.SetDefaultComparer(_defaultValueComparer);
                    foreach (var typeComparerOverride in _typeComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
                    }

                    foreach (var memberComparerOverride in _memberComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(memberComparerOverride.Key, memberComparerOverride.Value);
                    }

                    foreach (var failure in objectDataComparer.Compare(expectedValue, actualValue))
                    {
                        yield return failure.InsertPath(member.Name);
                    }

                    continue;
                }

                if (!comparer.Compare(expectedValue, actualValue))
                {
                    yield return new ComparisonFailure(member.Name, comparer.ToString(expectedValue), comparer.ToString(actualValue));
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
