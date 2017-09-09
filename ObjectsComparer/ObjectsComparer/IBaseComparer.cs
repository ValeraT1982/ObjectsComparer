using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public interface IBaseComparer
    {
        IValueComparer DefaultValueComparer { get; }

        ComparisonSettings Settings { get; }

        void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer);

        void AddComparerOverride(MemberInfo memberInfo, IValueComparer memberValueComparer);

        void AddComparerOverride(Type type, IValueComparer typeValueComparer);

        void AddComparerOverride<TType>(IValueComparer typeValueComparer);

        void SetDefaultComparer(IValueComparer valueComparer);

        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda, 
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction, 
            Func<TProp, string> toStringFunction);

        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda,
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction);
    }
}