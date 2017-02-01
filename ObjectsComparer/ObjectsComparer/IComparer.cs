using System.Collections.Generic;

namespace ObjectsComparer
{
    public interface IComparer
    {
        IEnumerable<ComparisonFailure> Compare(object expected, object actual);
    }
}