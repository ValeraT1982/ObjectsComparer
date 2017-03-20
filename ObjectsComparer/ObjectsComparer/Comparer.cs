using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class Comparer : AbstractComparer
    {
        private static string CalculateDifferencesMethodName
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            get { return MemberInfoExtensions.GetMethodName<Comparer<object>>(x => x.CalculateDifferences(null, null)); }
        }

        public Comparer(ComparisonSettings settings, IBaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            var genericType = typeof(Comparer<>).MakeGenericType(type);

            var comparer = Activator.CreateInstance(genericType, Settings, this, Factory);

            var method = genericType.GetTypeInfo().GetMethod(CalculateDifferencesMethodName, new[] { type, type });
            return (IEnumerable<Difference>)method.Invoke(comparer, new[] { obj1, obj2 });
        }
    }
}
