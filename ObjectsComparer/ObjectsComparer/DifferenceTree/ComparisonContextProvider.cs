using System;
using System.Reflection;

namespace ObjectsComparer
{
    public static class ComparisonContextProvider
    {
        public static IDifferenceTreeNode CreateRootContext()
        {
            return new DifferenceTreeNode(new DifferenceTreeNodeMember());
        }

        /// <summary>
        /// Returns the root of the difference tree for cases where consumers do not explicitly, directly or indirectly request the difference tree, this means that the difference tree is created only as an auxiliary.
        /// </summary>
        public static IDifferenceTreeNode CreateImplicitRootContext(ComparisonSettings comparisonSettings)
        {            
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));

            return new ImplicitDifferenceTreeNode(new DifferenceTreeNodeMember());
        }

        public static IDifferenceTreeNode CreateContext(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor)
        {
            return CreateContext(comparisonSettings, new DifferenceTreeNodeMember(), ancestor);
        }

        public static IDifferenceTreeNode CreateContext(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, MemberInfo memberInfo)
        {
            return CreateContext(comparisonSettings, new DifferenceTreeNodeMember(memberInfo, memberInfo?.Name), ancestor);
        }

        public static IDifferenceTreeNode CreateContext(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, string memberName)
        {
            return CreateContext(comparisonSettings, new DifferenceTreeNodeMember(name: memberName), ancestor);
        }

        public static IDifferenceTreeNode CreateContext(ComparisonSettings comparisonSettings, IDifferenceTreeNode ancestor, MemberInfo memberInfo, string memberName)
        {
            return CreateContext(comparisonSettings, new DifferenceTreeNodeMember(memberInfo, memberName), ancestor);
        }

        public static IDifferenceTreeNode CreateContext(ComparisonSettings comparisonSettings, IDifferenceTreeNodeMember comparisonContextMember, IDifferenceTreeNode ancestor)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));
            _ = comparisonContextMember ?? throw new ArgumentNullException(nameof(comparisonContextMember));
            //_ = ancestor ?? throw new ArgumentNullException(nameof(ancestor));

            ComparisonContextOptions options = ComparisonContextOptions.Default();
            comparisonSettings.ComparisonContextOptionsAction?.Invoke(ancestor, options);

            if (options.ComparisonContextMemberFactory != null)
            {
                var customComparisonContextMember = options.ComparisonContextMemberFactory.Invoke(comparisonContextMember);
                
                if (customComparisonContextMember == null)
                {
                    throw new InvalidOperationException("Comparison context member factory returned null member.");
                }

                comparisonContextMember = customComparisonContextMember;
            }

            if (options.ComparisonContextFactory != null)
            {
                var customContext = options.ComparisonContextFactory(comparisonContextMember);

                if (customContext != null)
                {
                    return customContext;
                }

                if (customContext == null)
                {
                    throw new InvalidOperationException("Comparison context factory returned null context.");
                }
            }

            return new DifferenceTreeNode(comparisonContextMember, ancestor);
        }
    }
}
