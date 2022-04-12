using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Root comparison context for cases where the consumer does not create and pass his own root context at the beginning of the comparison.
    /// </summary>
    internal class ImplicitDifferenceTreeNode : DifferenceTreeNodeBase
    {
        public ImplicitDifferenceTreeNode(IDifferenceTreeNodeMember member = null, IDifferenceTreeNode ancestor = null) : base(member, ancestor)
        {
        }

        public override void AddDifference(Difference difference)
        {
        }
    }
}
