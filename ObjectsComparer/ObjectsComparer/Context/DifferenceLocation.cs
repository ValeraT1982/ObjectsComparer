using System;

namespace ObjectsComparer.ContextExtensions
{
    /// <summary>
    /// The location of the difference in the difference tree.
    /// </summary>
    public class DifferenceLocation
    {
        public DifferenceLocation(Difference difference, IComparisonContext treeNode = null)
        {
            Difference = difference ?? throw new ArgumentNullException(nameof(difference));
            TreeNode = treeNode;
        }

        public Difference Difference { get; }
        
        public IComparisonContext TreeNode { get; }
    }
}