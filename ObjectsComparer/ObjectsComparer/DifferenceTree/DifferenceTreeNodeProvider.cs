using System;
using System.Reflection;

namespace ObjectsComparer
{
    public static class DifferenceTreeNodeProvider
    {
        public static IDifferenceTreeNode CreateRootNode()
        {
            return new DifferenceTreeNode(new DifferenceTreeNodeMember());
        }

        /// <summary>
        /// Returns the root of the difference tree for cases where the consumer does not explicitly, directly or indirectly request the difference tree, ie the difference tree is created only as an auxiliary.
        /// </summary>
        public static IDifferenceTreeNode CreateImplicitRootNode(ComparisonSettings comparisonSettings)
        {            
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));

            return new ImplicitDifferenceTreeNode(new DifferenceTreeNodeMember());
        }

        public static IDifferenceTreeNode CreateNode(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor)
        {
            return CreateNode(comparisonSettings, new DifferenceTreeNodeMember(), ancestor);
        }

        public static IDifferenceTreeNode CreateNode(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, MemberInfo memberInfo)
        {
            return CreateNode(comparisonSettings, new DifferenceTreeNodeMember(memberInfo, memberInfo?.Name), ancestor);
        }

        public static IDifferenceTreeNode CreateNode(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, string memberName)
        {
            return CreateNode(comparisonSettings, new DifferenceTreeNodeMember(name: memberName), ancestor);
        }

        public static IDifferenceTreeNode CreateNode(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, MemberInfo memberInfo, string memberName)
        {
            return CreateNode(comparisonSettings, new DifferenceTreeNodeMember(memberInfo, memberName), ancestor);
        }

        public static IDifferenceTreeNode CreateNode(ComparisonSettings comparisonSettings, IDifferenceTreeNodeMember differenceTreeNodeMember, IDifferenceTreeNode ancestor)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));
            _ = differenceTreeNodeMember ?? throw new ArgumentNullException(nameof(differenceTreeNodeMember));
            //_ = ancestor ?? throw new ArgumentNullException(nameof(ancestor));

            DifferenceTreeOptions options = DifferenceTreeOptions.Default();
            comparisonSettings.DifferenceTreeOptionsAction?.Invoke(ancestor, options);

            if (options.DifferenceTreeNodeMemberFactory != null)
            {
                var customDifferenceTreeNodeMember = options.DifferenceTreeNodeMemberFactory.Invoke(differenceTreeNodeMember);
                differenceTreeNodeMember = customDifferenceTreeNodeMember ?? throw new InvalidOperationException("Difference tree node member factory returned null member.");
            }

            if (options.DifferenceTreeNodeFactory != null)
            {
                var customDifferenceTreeNode = options.DifferenceTreeNodeFactory(differenceTreeNodeMember);

                if (customDifferenceTreeNode != null)
                {
                    return customDifferenceTreeNode;
                }

                if (customDifferenceTreeNode == null)
                {
                    throw new InvalidOperationException("Difference tree node factory returned null node.");
                }
            }

            return new DifferenceTreeNode(differenceTreeNodeMember, ancestor);
        }
    }
}
