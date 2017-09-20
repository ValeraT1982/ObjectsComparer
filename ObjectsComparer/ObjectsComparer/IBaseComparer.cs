using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public interface IBaseComparer
    {
        IValueComparer DefaultValueComparer { get; }

        ComparisonSettings Settings { get; }

        void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer valueComparer);

        void AddComparerOverride(MemberInfo memberInfo, IValueComparer valueComparer);

        void AddComparerOverride(Type type, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);

        void AddComparerOverride<TType>(IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);

        void SetDefaultComparer(IValueComparer valueComparer);

        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda, 
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction, 
            Func<TProp, string> toStringFunction);

        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda,
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction);

        void AddComparerOverride(string memberName, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);
    }
}