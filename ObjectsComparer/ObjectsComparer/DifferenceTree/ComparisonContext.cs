using System;

namespace ObjectsComparer
{
    public class ComparisonContext
    {
        public IDifferenceTreeNode RootNode { get; }

        public Difference CurrentDifference { get; }

        public IDifferenceTreeNode CurrentNode { get; }

        public ComparisonContext(IDifferenceTreeNode rootNode, Difference currentDifference, IDifferenceTreeNode currentNode = null)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
            CurrentDifference = currentDifference ?? throw new ArgumentNullException(nameof(currentDifference));
            CurrentNode = currentNode ?? throw new ArgumentNullException(nameof(currentNode));
        }
    }
}

