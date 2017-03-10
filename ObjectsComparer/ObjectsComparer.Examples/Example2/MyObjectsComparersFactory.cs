using System;

namespace ObjectsComparer.Examples.Example2
{
    public class MyObjectsComparersFactory: ObjectsComparersFactory
    {
        public override IObjectsDataComparer GetObjectsComparer(Type type, ComparisonSettings settings = null)
        {
            if (type == typeof(Customer))
            {
                var comparer = new ObjectsDataComparer<Customer>(settings);
                comparer.AddComparerOverride<Guid>(DoNotCompareValueComparer.Instance);
                comparer.AddComparerOverride(
                    () => new Customer().MiddleName,
                    new DynamicValueComparer<string>(
                        (s1, s2, parentSettings) => s1 == s2,
                        s => s));
                comparer.AddComparerOverride(
                    () => new Customer().PhoneNumber,
                    new PhoneNumberComparer());
            }

            return base.GetObjectsComparer(type, settings);
        }
    }
}
