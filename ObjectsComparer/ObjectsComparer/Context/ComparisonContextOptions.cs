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

        internal IComparisonContextFactory CustomComparisonContextFactory { get; private set; }

        public void UseComparisonContextFactory(IComparisonContextFactory comparisonContextFactory)
        {
            CustomComparisonContextFactory = comparisonContextFactory ?? throw new ArgumentNullException(nameof(comparisonContextFactory));
        }
    }
}
