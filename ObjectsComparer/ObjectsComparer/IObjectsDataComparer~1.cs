using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IObjectsDataComparer<T>: IObjectsDataComparer
    {
        IEnumerable<Difference> Compare(T obj1, T obj2);

        bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences);
    }
}