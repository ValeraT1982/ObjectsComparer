using System;
using System.Reflection;

namespace ObjectsComparer
{
    public static class ComparisonContextProvider
    {
        public static IComparisonContext CreateRootContext()
        {
            return new ComparisonContext(ComparisonContextMember.Create());
        }

        internal static IComparisonContext CreateImplicitRootContext(ComparisonSettings comparisonSettings)
        {            
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings)); //For future use.

            return new NullComparisonContext(ComparisonContextMember.Create());
        }              

        /// <summary>
        /// Context with ancestor but without a member, e.g. list element context.
        /// </summary>
        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor)
        {
            return CreateContext(comparisonSettings, ComparisonContextMember.Create(), ancestor);
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, MemberInfo member)
        {
            return CreateContext(comparisonSettings, ComparisonContextMember.Create(member), ancestor);
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, string memberName)
        {
            return CreateContext(comparisonSettings, ComparisonContextMember.Create(memberName), ancestor);
        }

        static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContextMember comparisonContextMember, IComparisonContext ancestor)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));
            _ = comparisonContextMember ?? throw new ArgumentNullException(nameof(comparisonContextMember));
            _ = ancestor ?? throw new ArgumentNullException(nameof(ancestor));

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

            return new ComparisonContext(comparisonContextMember, ancestor);
        }
    }
}
