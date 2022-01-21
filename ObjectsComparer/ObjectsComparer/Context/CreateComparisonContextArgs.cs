using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Arguments for <see cref="ComparisonContextOptions.UseComparisonContextFactory(System.Func{CreateComparisonContextArgs, IComparisonContext})"/> factory method. See <see cref="ComparisonContextOptions"/>.
    /// If all properties are null, the instance is probably intended to create a root context.
    /// </summary>
    public class CreateComparisonContextArgs
    {
        /// <summary>
        /// Context without ancestor and without member.
        /// </summary>
        internal CreateComparisonContextArgs()
        {
        }

        /// <summary>
        /// Context with ancestor but without a member.
        /// </summary>
        internal CreateComparisonContextArgs(IComparisonContext ancestor)
        {
            Ancestor = ancestor ?? throw new System.ArgumentNullException(nameof(ancestor));
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        internal CreateComparisonContextArgs(IComparisonContext ancestor, MemberInfo member)
        {
            Ancestor = ancestor ?? throw new System.ArgumentNullException(nameof(ancestor));
            Member = member ?? throw new System.ArgumentNullException(nameof(member));
        }

        /// <summary>
        /// Context with ancestor and member.
        /// </summary>
        internal CreateComparisonContextArgs(IComparisonContext ancestor, string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new System.ArgumentException($"'{nameof(memberName)}' cannot be null or empty.", nameof(memberName));
            }

            Ancestor = ancestor ?? throw new System.ArgumentNullException(nameof(ancestor));
            MemberName = memberName;
        }

        /// <summary>
        /// Nullable.
        /// </summary>
        public IComparisonContext Ancestor { get; }

        /// <summary>
        /// Nullable.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Nullable.
        /// </summary>
        public string MemberName { get; }
    }
}
