using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public abstract class AbstractComparer<T>: BaseComparer, IComparer<T>
    {
        protected AbstractComparer(ComparisonSettings settings, IBaseComparer parentComparer, IComparersFactory factory)
            :base(settings, parentComparer, factory)
        {
        }

        public bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences)
        {
            differences = CalculateDifferences(obj1, obj2);

            return !differences.Any();
        }

        public bool Compare(T obj1, T obj2)
        {
            return !CalculateDifferences(obj1, obj2).Any();
        }

        public abstract IEnumerable<Difference> CalculateDifferences(T obj1, T obj2);
    }
}