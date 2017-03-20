using System;

namespace ObjectsComparer
{
    public class ComparersFactory : IComparersFactory
    {
        public virtual IComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, IComparer parentComparer = null)
        {
            var comparer = typeof(Comparer<>).MakeGenericType(type);

            return (IComparer)Activator.CreateInstance(comparer, settings, parentComparer, this);
        }

        public IComparer GetObjectsComparer<T>(ComparisonSettings settings = null, IComparer parentComparer = null)
        {
            return GetObjectsComparer(typeof(T), settings, parentComparer);
        }
    }
}
