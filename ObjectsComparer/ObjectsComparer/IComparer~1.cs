using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IComparer<in T>: IBaseComparer
    {
        bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences);

        bool Compare(T obj1, T obj2);

        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2);
    }
}