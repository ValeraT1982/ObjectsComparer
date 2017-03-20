using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IComparer : IBaseComparer
    {
        bool Compare(Type type, object obj1, object obj2, out IEnumerable<Difference> differences);

        bool Compare(Type type, object obj1, object obj2);

        IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);
    }
}