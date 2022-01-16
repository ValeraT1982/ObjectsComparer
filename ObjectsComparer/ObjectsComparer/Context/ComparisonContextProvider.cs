using System;
using System.Reflection;

namespace ObjectsComparer
{
    internal static class ComparisonContextProvider
    {
        public static IComparisonContext CreateNullContext()
        {
            return new NullComparisonContext();
        }

        public static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, CreateComparisonContextArgs createContextArgs)
        {
            if (comparisonSettings is null)
            {
                throw new ArgumentNullException(nameof(comparisonSettings));
            }

            ComparisonContextOptions options = ComparisonContextOptions.Default();
            comparisonSettings.ComparisonContextOptionsAction?.Invoke(options);

            if (options.ComparisonContextFactory != null)
            {
                return options.ComparisonContextFactory(createContextArgs);
            }
            else
            {
                return new ComparisonContext(createContextArgs.Ancestor, CreateComparisonContextMember(createContextArgs.Member, createContextArgs.MemberName));
            }
        }

        static bool ShouldCreateNullContext(IComparisonContext ancestor)
        {
            do
            {
                if (ancestor is NullComparisonContext)
                {
                    return true;
                }

                ancestor = ancestor.Ancestor;

            } while (ancestor != null);

            return false;
        }

        static IComparisonContextMember CreateComparisonContextMember(MemberInfo member, string memberName)
        {
            if (member != null)
            {
                return new ComparisonContextMember(member);
            }

            if (string.IsNullOrWhiteSpace(memberName))
            {
                return new ComparisonContextMember();
            }

            return new ComparisonContextMember(memberName);
        }
    }
}
