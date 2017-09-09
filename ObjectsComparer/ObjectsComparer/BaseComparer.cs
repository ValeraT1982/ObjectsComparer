using System;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public abstract class BaseComparer: IBaseComparer
    {
        public ComparisonSettings Settings { get; }
        public IValueComparer DefaultValueComparer { get; private set; }

        protected IComparersFactory Factory { get; }

        internal ComparerOverridesCollection OverridesCollection { get;  } = new ComparerOverridesCollection();
        
        protected BaseComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
        {
            Factory = factory ?? new ComparersFactory();
            Settings = settings ?? new ComparisonSettings();
            DefaultValueComparer = new DefaultValueComparer();
            if (parentComparer != null)
            {
                DefaultValueComparer = parentComparer.DefaultValueComparer;
                OverridesCollection.Merge(parentComparer.OverridesCollection);
            }
        }

        public void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer)
        {
            OverridesCollection.AddComparer(PropertyHelper.GetMemberInfo(memberLambda), memberValueComparer);
        }

        public void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda, 
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction, 
            Func<TProp, string> toStringFunction)
        {
            OverridesCollection.AddComparer(
                PropertyHelper.GetMemberInfo(memberLambda),
                new DynamicValueComparer<TProp>(
                    compareFunction,
                    toStringFunction));
        }

        public void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda,
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction)
        {
            OverridesCollection.AddComparer(
                PropertyHelper.GetMemberInfo(memberLambda),
                new DynamicValueComparer<TProp>(
                    compareFunction,
                    obj => obj?.ToString()));
        }

        public void AddComparerOverride(MemberInfo memberInfo, IValueComparer memberValueComparer)
        {
            OverridesCollection.AddComparer(memberInfo, memberValueComparer);
        }

        public void AddComparerOverride(Type type, IValueComparer typeValueComparer)
        {
            OverridesCollection.AddComparer(type, typeValueComparer);
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