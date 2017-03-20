using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IComparer<in T>: IComparer
    {
        bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences);

        bool Compare(T obj1, T obj2);
    }
}