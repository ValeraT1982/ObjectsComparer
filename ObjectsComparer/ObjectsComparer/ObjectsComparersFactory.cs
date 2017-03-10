using System;

namespace ObjectsComparer
{
    public class ObjectsComparersFactory : IObjectsComparersFactory
    {
        public virtual IObjectsDataComparer GetObjectsComparer(Type type, ComparisonSettings settings = null)
        {
            return ObjectsDataComparer<object>.CreateComparer(type, settings ?? new ComparisonSettings());
        }

        public IObjectsDataComparer GetObjectsComparer<T>(ComparisonSettings settings = null)
        {
            return GetObjectsComparer(typeof(T), settings);
        }
    }
}
