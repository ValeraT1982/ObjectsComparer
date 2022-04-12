using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// The root difference tree node for cases where consumers does not explicitly, directly or indirectly request a difference tree, this means that the difference tree will only be created as an auxiliary.
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
