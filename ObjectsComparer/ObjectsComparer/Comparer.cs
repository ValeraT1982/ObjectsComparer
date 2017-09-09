using System;
using System.Collections.Generic;
using System.Linq;
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

        public Comparer(ComparisonSettings settings = null, BaseComparer parentComparer = null, IComparersFactory factory = null) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2)
        {
            var objectsComparerMethod = typeof(IComparersFactory).GetTypeInfo().GetMethods().First(m => m.IsGenericMethod);
            var objectsComparerGenericMethod = objectsComparerMethod.MakeGenericMethod(type);
            var comparer = objectsComparerGenericMethod.Invoke(Factory, new object[] { Settings, this });
            var genericType = typeof(IComparer<>).MakeGenericType(type);
            var method = genericType.GetTypeInfo().GetMethod(CalculateDifferencesMethodName, new[] { type, type });

            return (IEnumerable<Difference>)method.Invoke(comparer, new[] { obj1, obj2 });
        }
    }
}
