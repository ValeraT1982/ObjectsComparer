using System;

namespace ObjectsComparer.Examples.Example2
{
    public class MyObjectsComparersFactory: ObjectsComparersFactory
    {
        public override IObjectsDataComparer GetObjectsComparer(Type type, ComparisonSettings settings = null)
        {
            if (type == typeof(Person))
            {
                var comparer = new ObjectsDataComparer<Person>(settings);
                comparer.AddComparerOverride<Guid>(DoNotCompareValueComparer.Instance);
                comparer.AddComparerOverride(
                    () => new Person().MiddleName,
                    new DynamicValueComparer<string>(
                        (s1, s2, parentSettings) => string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2) || s1 == s2,
                        s => s));
                comparer.AddComparerOverride(
                    () => new Person().PhoneNumber,
                    new PhoneNumberComparer());

                return comparer;
            }

            return base.GetObjectsComparer(type, settings);
        }
    }
}
