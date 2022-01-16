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

        internal Func<CreateComparisonContextArgs, IComparisonContext> ComparisonContextFactory { get; private set; }

        public void UseComparisonContextFactory(Func<CreateComparisonContextArgs, IComparisonContext> factory)
        {
            ComparisonContextFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}
