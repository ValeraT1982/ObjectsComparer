using System;

namespace ObjectsComparer
{
    public class ObjectsComparersFactory : IObjectsComparersFactory
    {
        public virtual IObjectsDataComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, IObjectsDataComparer parentComparer = null)
        {
            var comparer = typeof(ObjectsDataComparer<>).MakeGenericType(type);

            return (IObjectsDataComparer)Activator.CreateInstance(comparer, settings, parentComparer, this);
        }

        public IObjectsDataComparer GetObjectsComparer<T>(ComparisonSettings settings = null, IObjectsDataComparer parentComparer = null)
        {
            return GetObjectsComparer(typeof(T), settings, parentComparer);
        }
    }
}
