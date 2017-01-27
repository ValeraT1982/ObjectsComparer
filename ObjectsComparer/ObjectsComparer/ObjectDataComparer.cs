using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public class ObjectDataComparer<T> : IObjectDataComparer
    {
        public bool SkipDefaultValues { get; set; }
        public bool RecursiveComparison { get; set; }

        private readonly Dictionary<PropertyInfo, IComparer> _propertyComparerOverrides = new Dictionary<PropertyInfo, IComparer>();
        private readonly Dictionary<Type, IComparer> _typeComparerOverrides = new Dictionary<Type, IComparer>();
        private readonly List<PropertyInfo> _properties;
        private IComparer _defaultComparer;

        public ObjectDataComparer()
        {
            _properties = typeof(T).GetProperties().Where(p =>
                p.CanRead
                && p.GetGetMethod(true).IsPublic
                && p.GetGetMethod(true).GetParameters().Length == 0
                && !p.GetGetMethod(true).IsStatic).ToList();

            SkipDefaultValues = true;
            RecursiveComparison = true;
            _defaultComparer = new DefaultComparer();
            AddComparerOverride(typeof(string), new NulableStringsComparer());
        }


        public void AddComparerOverride<TProp>(Expression<Func<TProp>> propertyLambda, IComparer propertyComparer)
        {
            _propertyComparerOverrides[PropertyHelper.GetPropertyInfo(propertyLambda)] = propertyComparer;
        }

        public void AddComparerOverride(PropertyInfo propertyInfo, IComparer propertyComparer)
        {
            _propertyComparerOverrides[propertyInfo] = propertyComparer;
        }

        public void AddComparerOverride(Type type, IComparer typeComparer)
        {
            _typeComparerOverrides[type] = typeComparer;
        }

        public void SetDefaultComparer(IComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            _defaultComparer = comparer;
        }

        public IEnumerable<ComparisonFailure> Compare(object expectedObject, object actualObject)
        {
            return Compare((T)expectedObject, (T)actualObject);
        }

        public IEnumerable<ComparisonFailure> Compare(T expectedObject, T actualObject)
        {
            foreach (var prop in _properties)
            {
                var expectedValue = prop.GetValue(expectedObject);
                var actualValue = prop.GetValue(actualObject);

                if (SkipDefaultValues && expectedValue.IsDefaultValue())
                {
                    continue;
                }

                var comparer = _defaultComparer;
                var hasCustomComparer = false;

                if (_propertyComparerOverrides.ContainsKey(prop))
                {
                    comparer = _propertyComparerOverrides[prop];
                    hasCustomComparer = true;
                }
                else if (_typeComparerOverrides.ContainsKey(prop.PropertyType))
                {
                    comparer = _typeComparerOverrides[prop.PropertyType];
                    hasCustomComparer = true;
                }

                if (RecursiveComparison
                    && !hasCustomComparer
                    && expectedValue != null
                    && actualValue != null
                    && prop.PropertyType.IsClass
                    && prop.PropertyType != typeof(string)
                    && !prop.PropertyType.IsAssignableFrom(typeof(IComparable))
                    && !prop.PropertyType.IsAssignableFrom(typeof(IComparable<>)))
                {
                    var objectDataComparerType = typeof(ObjectDataComparer<>).MakeGenericType(prop.PropertyType);
                    var objectDataComparer = (IObjectDataComparer)Activator.CreateInstance(objectDataComparerType);

                    objectDataComparer.SkipDefaultValues = SkipDefaultValues;
                    objectDataComparer.RecursiveComparison = true;
                    objectDataComparer.SetDefaultComparer(_defaultComparer);
                    foreach (var typeComparerOverride in _typeComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(typeComparerOverride.Key, typeComparerOverride.Value);
                    }

                    foreach (var propertyComparerOverride in _propertyComparerOverrides)
                    {
                        objectDataComparer.AddComparerOverride(propertyComparerOverride.Key, propertyComparerOverride.Value);
                    }

                    foreach (var failure in objectDataComparer.Compare(expectedValue, actualValue))
                    {
                        yield return failure;
                    }

                    continue;
                }

                if (!comparer.Compare(expectedValue, actualValue))
                {
                    yield return new ComparisonFailure
                    {
                        ExpectedValue = comparer.ToString(expectedValue),
                        ActualValue = comparer.ToString(actualValue),
                        PropertyName = prop.Name,
                    };
                }
            }
        }
    }
}
