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

        /// <summary>
        /// Creates <see cref="IComparisonContext"/>.
        /// </summary>
        /// <param name="factory">If factory returns null, the default comparison context will be used.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UseComparisonContextFactory(Func<CreateComparisonContextArgs, IComparisonContext> factory)
        {
            ComparisonContextFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}
