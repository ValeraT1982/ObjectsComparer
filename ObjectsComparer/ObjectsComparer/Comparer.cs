using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Compares objects.
    /// </summary>
    public class Comparer : AbstractComparer
    {
        private static string CalculateDifferencesMethodName
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            get { return MemberInfoExtensions.GetMethodName<Comparer<object>>(x => x.CalculateDifferences(null, null)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer" /> class. 
        /// </summary>
        /// <param name="settings">Comparison Settings.</param>
        /// <param name="parentComparer">Parent Comparer. Is used to copy DefaultValueComparer and Overrides. Null by default.</param>
        /// <param name="factory">Factory to create comparers in case of some members of the objects will need it.</param>
        public Comparer(ComparisonSettings settings = null, BaseComparer parentComparer = null, IComparersFactory factory = null) : base(settings, parentComparer, factory)
        {
        }

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
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
