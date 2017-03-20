using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public abstract class AbstractComparer: BaseComparer, IComparer
    {
        protected AbstractComparer(ComparisonSettings settings, IBaseComparer parentComparer, IComparersFactory factory) 
            : base(settings, parentComparer, factory)
        {
        }

        public abstract IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);

        public bool Compare(Type type, object obj1, object obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(type, obj1, obj2);

            return !differences.Any();
        }

        public bool Compare(Type type, object obj1, object obj2)
        {
            return !CalculateDifferences(type, obj1, obj2).Any();
        }
    }
}