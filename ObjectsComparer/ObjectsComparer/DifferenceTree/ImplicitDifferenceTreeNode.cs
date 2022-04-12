using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// The root of the difference tree for cases where consumers do not explicitly, directly or indirectly request the difference tree, this means that the difference tree is created only as an auxiliary.
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
