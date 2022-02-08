using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class ComparisonContextOptions
    {
        ComparisonContextOptions()
        {
        }

        internal static ComparisonContextOptions Default()
        {
            return new ComparisonContextOptions();
        }

        internal Func<IComparisonContextMember, IComparisonContext> ComparisonContextFactory { get; private set; }

        internal Func<IComparisonContextMember, IComparisonContextMember> ComparisonContextMemberFactory { get; private set; }
        
        /// <summary>
        /// Factory for <see cref="IComparisonContext"/> instances.
        /// </summary>
        /// <param name="factory"></param>
        public void UseComparisonContextFactory(Func<IComparisonContextMember, IComparisonContext> factory)
        {
            ComparisonContextFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Factory for <see cref="IComparisonContextMember"/> instances.
        /// </summary>
        public void UseComparisonContextMemberFactory(Func<IComparisonContextMember, IComparisonContextMember> factory)
        {
            ComparisonContextMemberFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public bool ThrowContextableComparerNotImplementedEnabled { get; private set; } = true;

        /// <summary>
        /// Whether to throw an <see cref="Exceptions.ContextableComparerNotImplementedException"/> if the custom comparer does not implement <see cref="IContextableComparer"/> or <see cref="IContextableComparer{T}"/>.
        /// </summary>
        public ComparisonContextOptions ThrowContextableComparerNotImplemented(bool value)
        {
            ThrowContextableComparerNotImplementedEnabled = false;

            return this;
        }
    }
}
