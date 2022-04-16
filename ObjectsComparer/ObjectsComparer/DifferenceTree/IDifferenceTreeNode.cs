using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ObjectsComparer
{
    //TODO: Edit "Once the comparison process is completed, it is possible to traverse "

    /// <summary>
    /// Node in the difference tree. Each instance of the node wraps compared <see cref="Member"/>, which is typically property. Each node has its ancestor and descendants the same way as its compared <see cref="Member"/> has its ancestor and descendant members.
    /// Once the comparison process is finished (completed or uncompleted), it is possible to traverse the difference tree and see differences at particular members.
    /// </summary>
    public interface IDifferenceTreeNode
    {
        /// <summary>
        /// Ancestor node.
        /// </summary>
        IDifferenceTreeNode Ancestor { get; set; }

        /// <summary>
        /// Children nodes.
        /// </summary>
        IEnumerable<IDifferenceTreeNode> Descendants { get; }

        /// <summary>
        /// A list of differences directly related to the node.
        /// </summary>
        IEnumerable<Difference> Differences { get; }

        /// <summary>
        /// Compared member, for more info see <see cref="IDifferenceTreeNodeMember"/>.
        /// It should be null for the root node (the starting point of the comparison) and for the list element node. A list element node never has a member, but it has an ancestor node which is the list and that list has its member.
        /// </summary>
        IDifferenceTreeNodeMember Member { get; }

        /// <summary>
        /// Adds descendant to the node.
        /// </summary>
        /// <param name="descendant"></param>
        void AddDescendant(IDifferenceTreeNode descendant);

        /// <summary>
        /// Adds the difference to the node.
        /// </summary>
        /// <param name="difference"></param>
        void AddDifference(Difference difference);

        /// <summary>
        /// Returns differences directly or indirectly related to the node.
        /// </summary>
        /// <param name="recursive">If value is true, it also looks for <see cref="Differences"/> in <see cref="Descendants"/>.</param>
        IEnumerable<Difference> GetDifferences(bool recursive = true);

        /// <summary>
        /// Whether there are differences directly or indirectly related to the node.
        /// </summary>
        /// <param name="recursive">If value is true, it also looks for <see cref="Differences"/> in <see cref="Descendants"/>.</param>
        bool HasDifferences(bool recursive);

        /// <summary>
        /// Removes all <see cref="Descendants"/> which have no <see cref="Differences"/> directly or indirectly in their <see cref="Descendants"/>.
        /// </summary>
        IDifferenceTreeNode Shrink();
    }
}
