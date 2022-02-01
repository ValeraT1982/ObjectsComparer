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
        

        public void UseComparisonContextFactory(Func<IComparisonContextMember, IComparisonContext> factory)
        {
            ComparisonContextFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// </summary>
        /// <param name="factory">First parameter type: Default member.</param>
        public void UseComparisonContextMemberFactory(Func<IComparisonContextMember, IComparisonContextMember> factory)
        {
            ComparisonContextMemberFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}
