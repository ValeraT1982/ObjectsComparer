using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public abstract class AbstractObjectsDataComparer<T> : IObjectsDataComparer<T>
    {
        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();

        protected IObjectsComparersFactory Factory { get; }

        public ComparisonSettings Settings { get; }
        public IEnumerable<KeyValuePair<MemberInfo, IValueComparer>> MemberComparerOverrides => _memberComparerOverrides.Select(o => o);
        public IEnumerable<KeyValuePair<Type, IValueComparer>> TypeComparerOverrides => _typeComparerOverrides.Select(o => o);
        public IValueComparer DefaultValueComparer { get; private set; }

        protected AbstractObjectsDataComparer(ComparisonSettings settings, IObjectsDataComparer parentComparer, IObjectsComparersFactory factory)
        {
            Factory = factory ?? new ObjectsComparersFactory();
            Settings = settings ?? new ComparisonSettings();
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
            if (!(obj1 is T))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!(obj2 is T))
            {
                throw new ArgumentException(nameof(obj2));
            }

            return Compare((T) obj1, (T) obj2, out differences);
        }

        public bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(obj1, obj2);

            return !differences.Any();
        }

        public bool Compare(object obj1, object obj2)
        {
            if (!(obj1 is T))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!(obj2 is T))
            {
                throw new ArgumentException(nameof(obj2));
            }

            return !CalculateDifferences((T)obj1, (T)obj2).Any();
        }

        public bool Compare(T obj1, T obj2)
        {
            return !CalculateDifferences(obj1, obj2).Any();
        }

        public abstract IEnumerable<Difference> CalculateDifferences(object obj1, object obj2);
    }
}