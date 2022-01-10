using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
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
