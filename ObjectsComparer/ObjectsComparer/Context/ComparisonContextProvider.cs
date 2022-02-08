using System;
using System.Reflection;

namespace ObjectsComparer
{
    public static class ComparisonContextProvider
    {
        public static IComparisonContext CreateRootContext()
        {
            return new ComparisonContext(new ComparisonContextMember());
        }

        internal static IComparisonContext CreateImplicitRootContext(ComparisonSettings comparisonSettings)
        {            
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));

            return new ImplicitRootComparisonContext(new ComparisonContextMember());
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor)
        {
            return CreateContext(comparisonSettings, new ComparisonContextMember(), ancestor);
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, MemberInfo memberInfo)
        {
            return CreateContext(comparisonSettings, new ComparisonContextMember(memberInfo, memberInfo?.Name), ancestor);
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, string memberName)
        {
            return CreateContext(comparisonSettings, new ComparisonContextMember(name: memberName), ancestor);
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, MemberInfo memberInfo, string memberName)
        {
            return CreateContext(comparisonSettings, new ComparisonContextMember(memberInfo, memberName), ancestor);
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, IComparisonContextMember comparisonContextMember, IComparisonContext ancestor)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));
            _ = comparisonContextMember ?? throw new ArgumentNullException(nameof(comparisonContextMember));
            _ = ancestor ?? throw new ArgumentNullException(nameof(ancestor));

            ComparisonContextOptions options = ComparisonContextOptions.Default();
            comparisonSettings.ComparisonContextOptionsAction?.Invoke(new ComparisonContextInfo(ancestor), options);

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
