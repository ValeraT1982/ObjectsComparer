using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public abstract class BaseComparer
    {
        private readonly Dictionary<MemberInfo, IValueComparer> _memberComparerOverrides = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, IValueComparer> _typeComparerOverrides = new Dictionary<Type, IValueComparer>();

        protected BaseComparer(ComparisonSettings settings, IBaseComparer parentComparer, IComparersFactory factory)
        {
            Factory = factory ?? new ComparersFactory();
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

        public ComparisonSettings Settings { get; }
        public IEnumerable<KeyValuePair<MemberInfo, IValueComparer>> MemberComparerOverrides => Enumerable.Select<KeyValuePair<MemberInfo, IValueComparer>, KeyValuePair<MemberInfo, IValueComparer>>(_memberComparerOverrides, o => o);
        public IEnumerable<KeyValuePair<Type, IValueComparer>> TypeComparerOverrides => Enumerable.Select<KeyValuePair<Type, IValueComparer>, KeyValuePair<Type, IValueComparer>>(_typeComparerOverrides, o => o);
        public IValueComparer DefaultValueComparer { get; private set; }
        protected IComparersFactory Factory { get; }

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
    }
}