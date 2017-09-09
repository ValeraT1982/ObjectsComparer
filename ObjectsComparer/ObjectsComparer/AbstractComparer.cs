using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public abstract class AbstractComparer: BaseComparer, IComparer
    {
        protected AbstractComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) 
            : base(settings, parentComparer, factory)
        {
        }

        public abstract IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);

        public IEnumerable<Difference> CalculateDifferences<T>(T obj1, T obj2)
        {
            return CalculateDifferences(typeof(T), obj1, obj2);
        }

        public bool Compare(Type type, object obj1, object obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(type, obj1, obj2);

            return !differences.Any();
        }

        public bool Compare(Type type, object obj1, object obj2)
        {
            return !CalculateDifferences(type, obj1, obj2).Any();
        }

        public bool Compare<T>(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            return Compare(typeof(T), obj1, obj2, out differences);
        }

        public bool Compare<T>(T obj1, T obj2)
        {
            return Compare(typeof(T), obj1, obj2);
        }
    }
}