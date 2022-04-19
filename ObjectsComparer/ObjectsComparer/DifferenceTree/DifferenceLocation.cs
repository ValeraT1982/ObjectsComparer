using System;

namespace ObjectsComparer.DifferenceTreeExtensions
{
    /// <summary>
    /// The location of the difference in the difference tree.
    /// </summary>
    public class DifferenceLocation
    {
        public DifferenceLocation(Difference difference, IDifferenceTreeNode treeNode = null)
        {
            Difference = difference ?? throw new ArgumentNullException(nameof(difference));
            TreeNode = treeNode;
        }

        public Difference Difference { get; }

        /// <summary>
        /// Optional. Returns null, if no location is specified (probably by a builder who does not implement <see cref="IDifferenceTreeBuilder"/>).
        /// </summary>
        public IDifferenceTreeNode TreeNode { get; }
    }
}