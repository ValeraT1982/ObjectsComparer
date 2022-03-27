using System;

namespace ObjectsComparer.ContextExtensions
{
    /// <summary>
    /// The location of the <see cref="Difference"/> in the difference tree.
    /// </summary>
    public class DifferenceTreeNodeInfo
    {
        public DifferenceTreeNodeInfo(Difference difference, IComparisonContext treeNode = null)
        {
            Difference = difference ?? throw new ArgumentNullException(nameof(difference));
            TreeNode = treeNode;
        }

        public Difference Difference { get; }

        public IComparisonContext TreeNode { get; }
    }
}