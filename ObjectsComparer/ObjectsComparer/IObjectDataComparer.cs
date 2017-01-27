using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public interface IObjectDataComparer
    {
        bool SkipDefaultValues { get; set; }
        bool RecursiveComparison { get; set; }
        void AddComparerOverride<TProp>(Expression<Func<TProp>> propertyLambda, IComparer propertyComparer);
        void AddComparerOverride(PropertyInfo propertyInfo, IComparer propertyComparer);
        void AddComparerOverride(Type type, IComparer typeComparer);
        void SetDefaultComparer(IComparer comparer);
        IEnumerable<ComparisonFailure> Compare(object expectedObject, object actualObject);
    }
}