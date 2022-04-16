using System;

namespace ObjectsComparer
{
    /// <summary>
    /// The current context of the comparison in the process of creating the difference tree. For more info, see <see cref="ComparerExtensions.CalculateDifferenceTree(IComparer, Type, object, object, Func{ComparisonContext, bool}, Action)"/>, or <see cref="ComparerExtensions.CalculateDifferenceTree{T}(IComparer{T}, T, T, Func{ComparisonContext, bool}, Action)"/>.
    /// </summary>
    public class ComparisonContext
    {
        public IDifferenceTreeNode RootNode { get; }

        public Difference CurrentDifference { get; }

        public IDifferenceTreeNode CurrentNode { get; }

        public ComparisonContext(IDifferenceTreeNode rootNode, Difference currentDifference, IDifferenceTreeNode currentNode = null)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
            CurrentDifference = currentDifference ?? throw new ArgumentNullException(nameof(currentDifference));
            CurrentNode = currentNode;
        }
    }
}

