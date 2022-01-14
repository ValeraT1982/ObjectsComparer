using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Null context for cases where the consumer does not create his own root context at the beginning of the comparison. No descendants or differences will be builded in the comparison process.
    /// </summary>
    internal class NullComparisonContext : ComparisonContextBase
    {
        public NullComparisonContext(IComparisonContextMember member = null, IComparisonContext ancestor = null) : base(member, ancestor)
        {
        }

        public override void AddDifference(Difference difference)
        {
        }
    }
}
