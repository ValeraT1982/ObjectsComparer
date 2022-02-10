using System;
using System.Reflection;
using ObjectsComparer.Exceptions;

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
        /// Whether to throw the <see cref="ContextableComparerNotImplementedException"/> when the user requires comparison context but has a comparer that does not implement <see cref="IContextableComparer"/> or <see cref="IContextableComparer{T}"/>.
        /// Default = true.
        /// </summary>
        public ComparisonContextOptions ThrowContextableComparerNotImplemented(bool value)
        {
            ThrowContextableComparerNotImplementedEnabled = value;

            return this;
        }
    }
}
