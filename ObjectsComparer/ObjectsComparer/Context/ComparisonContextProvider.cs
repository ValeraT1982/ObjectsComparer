using System;
using System.Reflection;

namespace ObjectsComparer
{
    internal static class ComparisonContextProvider
    {
        internal static IComparisonContext CreateImplicitRootContext(ComparisonSettings comparisonSettings)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings)); //For the future.

            return new NullComparisonContext();
        }              

        /// <summary>
        /// Context without ancestor and without member.
        /// </summary>
        public static IComparisonContext CreateRootContext(ComparisonSettings comparisonSettings)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));

            return CreateContext(comparisonSettings, new CreateComparisonContextArgs());
        }

        /// <summary>
        /// Context with ancestor but without a member.
        /// </summary>
        public static IComparisonContext CreateListElementContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor)
        {
            return CreateContext(comparisonSettings, new CreateComparisonContextArgs(ancestor));
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        public static IComparisonContext CreateMemberContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, MemberInfo member)
        {
            return CreateContext(comparisonSettings, new CreateComparisonContextArgs(ancestor, member));
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        public static IComparisonContext CreateMemberNameContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, string memberName)
        {
            return CreateContext(comparisonSettings, new CreateComparisonContextArgs(ancestor, memberName));
        }

        /// <summary>
        /// Context with ancestor and member. The <paramref name="member"/> takes precedence over <paramref name="memberName"/>.
        /// </summary>
        internal static IComparisonContext CreateMemberOrMemberNameContext(ComparisonSettings comparisonSettings, IComparisonContext ancestor, MemberInfo member, string memberName)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));

            if (member != null)
            {
                return CreateMemberContext(comparisonSettings, ancestor, member);
            }

            if (memberName != null)
            {
                return CreateMemberNameContext(comparisonSettings, ancestor, memberName);
            }

            if (ancestor != null)
            {
                return CreateListElementContext(comparisonSettings, ancestor);
            }

            return CreateRootContext(comparisonSettings);
        }

        static IComparisonContext CreateContext(ComparisonSettings comparisonSettings, CreateComparisonContextArgs createContextArgs)
        {
            _ = comparisonSettings ?? throw new ArgumentNullException(nameof(comparisonSettings));
            _ = createContextArgs ?? throw new ArgumentNullException(nameof(createContextArgs));

            ComparisonContextOptions options = ComparisonContextOptions.Default();
            comparisonSettings.ComparisonContextOptionsAction?.Invoke(options);

            if (options.ComparisonContextFactory != null)
            {
                var context = options.ComparisonContextFactory(createContextArgs);
                if (context != null)
                {
                    return context;
                }
            }

            return new ComparisonContext(CreateComparisonContextMember(createContextArgs.Member, createContextArgs.MemberName), createContextArgs.Ancestor);
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
