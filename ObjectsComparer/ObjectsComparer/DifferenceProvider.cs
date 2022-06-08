using System;

namespace ObjectsComparer
{
    public static class DifferenceProvider
    {
        public static Difference CreateDifference(ComparisonSettings settings, IDifferenceTreeNode differenceTreeNode, Difference sourceDifference)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            if (sourceDifference is null)
            {
                throw new ArgumentNullException(nameof(sourceDifference));
            }

            var options = DifferenceOptions.Default();

            settings.DifferenceOptionsAction?.Invoke(differenceTreeNode, options);

            var customDifference = options.DifferenceFactory(sourceDifference);

            return customDifference;
        }
    }
}
