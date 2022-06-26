using System;

namespace ObjectsComparer
{
    public static class DifferenceProvider
    {
        public static Difference CreateDifference(ComparisonSettings settings, IDifferenceTreeNode differenceTreeNode, Difference defaultDifference, object rawValue1, object rawValue2)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            if (defaultDifference is null)
            {
                throw new ArgumentNullException(nameof(defaultDifference));
            }

            var differenceOptions = DifferenceOptions.Default();

            settings.DifferenceOptionsAction?.Invoke(differenceTreeNode, differenceOptions);

            if (differenceOptions.DifferenceFactory == null)
            {
                return defaultDifference;
            }

            return differenceOptions.DifferenceFactory(new CreateDifferenceArgs(defaultDifference, rawValue1, rawValue2));
        }
    }
}
