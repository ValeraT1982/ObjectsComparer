using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IEnumerablesComparer
    {
        IEnumerable<Difference> Compare(object obj1, object obj2);
    }
}