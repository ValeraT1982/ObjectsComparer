using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Root compariosin context for cases where the consumer does not create and pass his own root context at the beginning of the comparison.
    /// </summary>
    internal class ImplicitRootComparisonContext : ComparisonContextBase
    {
        public ImplicitRootComparisonContext(IComparisonContextMember member = null, IComparisonContext ancestor = null) : base(member, ancestor)
        {
        }

        public override void AddDifference(Difference difference)
        {
        }
    }
}
