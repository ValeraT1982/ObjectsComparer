using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface ICollectionsComparer
    {
        IEnumerable<Difference> Compare(object obj1, object obj2);
    }
}