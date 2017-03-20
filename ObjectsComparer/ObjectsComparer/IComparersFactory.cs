using System;

namespace ObjectsComparer
{
    public interface IComparersFactory
    {
        IComparer GetObjectsComparer(Type type, ComparisonSettings settings = null, IComparer parentComparer = null);

        IComparer GetObjectsComparer<T>(ComparisonSettings settings = null, IComparer parentComparer = null);
    }
}