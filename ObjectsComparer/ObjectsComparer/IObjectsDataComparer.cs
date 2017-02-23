using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public interface IObjectsDataComparer
    {
        void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer memberValueComparer);

        void AddComparerOverride(MemberInfo memberInfo, IValueComparer memberValueComparer);

        void AddComparerOverride(Type type, IValueComparer typeValueComparer);

        void SetDefaultComparer(IValueComparer valueComparer);
        
        IEnumerable<Difference> Compare(object obj1, object obj2);

        void ConfigureChildComparer(IObjectsDataComparer comparer);
    }
}