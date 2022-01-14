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

        /// <summary>
        /// <paramref name="member"/> takes precedence over <paramref name="memberName"/>.
        /// </summary>
        public static IComparisonContext CreateContext(ComparisonContextOptions options, IComparisonContext ancestor, MemberInfo member, string memberName)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (ShouldCreateNullContext(ancestor))
            {
                return new NullComparisonContext(null, null);
            }

            var ancestorInfo = ancestor != null ? new ComparisonContextInfo(ancestor) : null;

            if (options.CustomComparisonContextFactory != null)
            {
                if (member != null)
                {
                    return options.CustomComparisonContextFactory.CreateContext(member, ancestorInfo);
                }

                if (string.IsNullOrWhiteSpace(memberName))
                {
                    return options.CustomComparisonContextFactory.CreateContext(memberName, ancestorInfo);
                }

                return options.CustomComparisonContextFactory.CreateContext(ancestorInfo);
            }

            return new ComparisonContext(CreateComparisonContextMember(memberName, member), ancestor);
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

        static IComparisonContextMember CreateComparisonContextMember(string memberName, MemberInfo member)
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
