using System;

namespace ObjectsComparer
{
    public class ComparisonContextOptions
    {
        bool _comparisonContextDisabled = false;

        IComparisonContextFactory _customComparisonContextFactory;

        IComparisonContextFactory _defaultComparisonContextFactory = new DefaultComparisonContextFactory();

        IComparisonContextFactory _nullComparisonContextFactory = new NullComparisonContextFactory();

        public void UseComparisonContextFactory(IComparisonContextFactory comparisonContextFactory)
        {
            _customComparisonContextFactory = comparisonContextFactory ?? throw new ArgumentNullException(nameof(comparisonContextFactory));
        }

        internal void DisableComparisonContext()
        {
            _comparisonContextDisabled = true;
        }

        internal IComparisonContextFactory GetComparisonContextFactory(IComparisonContext ancestor = null)
        {
            do
            {
                if (ancestor is NullComparisonContext)
                {
                    return _nullComparisonContextFactory;
                }

                ancestor = ancestor.Ancestor;

            } while (ancestor != null);

            if (_customComparisonContextFactory != null)
            {
                return _customComparisonContextFactory;
            }

            return _defaultComparisonContextFactory;
        }
    }
}
