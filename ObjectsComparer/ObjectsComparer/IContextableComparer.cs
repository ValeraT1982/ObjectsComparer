using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IContextableComparer
    {
        IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2, ComparisionContext comparisionContext);
    }

    
}