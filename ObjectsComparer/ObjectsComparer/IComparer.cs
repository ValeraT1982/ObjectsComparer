using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public interface IComparer
    {
        IEnumerable<KeyValuePair<MemberInfo, IValueComparer>> MemberComparerOverrides { get; }

        IEnumerable<KeyValuePair<Type, IValueComparer>> TypeComparerOverrides { get; }

        IValueComparer DefaultValueComparer { get; }

        ComparisonSettings Settings { get; }

        void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer);

        void AddComparerOverride(MemberInfo memberInfo, IValueComparer memberValueComparer);

        void AddComparerOverride(Type type, IValueComparer typeValueComparer);

        void AddComparerOverride<TType>(IValueComparer typeValueComparer);

        void SetDefaultComparer(IValueComparer valueComparer);

        bool Compare(object obj1, object obj2, out IEnumerable<Difference> differences);

        bool Compare(object obj1, object obj2);

        IEnumerable<Difference> CalculateDifferences(object obj1, object obj2);
    }
}