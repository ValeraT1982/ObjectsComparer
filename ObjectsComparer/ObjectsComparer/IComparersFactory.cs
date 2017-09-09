using System;

namespace ObjectsComparer
{
    public interface IComparersFactory
    {
        IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, BaseComparer parentComparer = null);

        IComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, BaseComparer parentComparer = null);
    }
}