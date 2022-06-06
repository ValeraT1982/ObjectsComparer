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

            var customDifference = sourceDifference;

            if (options.DifferenceFactory != null)
            {
                customDifference = options.DifferenceFactory(sourceDifference);
            }
            else
            {
                if (options.RawValuesIncluded != true && (sourceDifference.RawValue1 != null || sourceDifference.RawValue2 != null)) 
                {
                    customDifference = new Difference(
                        memberPath: sourceDifference.MemberPath,
                        value1: sourceDifference.Value1,
                        value2: sourceDifference.Value2,
                        differenceType: DifferenceTypes.TypeMismatch);
                }
            }

            return customDifference;
        }
    }
}
