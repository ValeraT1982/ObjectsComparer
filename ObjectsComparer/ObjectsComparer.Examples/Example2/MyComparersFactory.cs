using System;

namespace ObjectsComparer.Examples.Example2
{
    public class MyComparersFactory : ComparersFactory
    {
        public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null,
            BaseComparer parentComparer = null)
        {
            if (typeof(T) == typeof(Person))
            {
                var comparer = new Comparer<Person>(settings, parentComparer, this);
                //Do not compare PersonId
                comparer.AddComparerOverride<Guid>(DoNotCompareValueComparer.Instance);
                //Sometimes MiddleName can be skipped. Compare only if property has value.
                comparer.AddComparerOverride(
                    () => new Person().MiddleName,
                    (s1, s2, parentSettings) => string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2) || s1 == s2);
                comparer.AddComparerOverride(
                    () => new Person().PhoneNumber,
                    new PhoneNumberComparer());

                return (IComparer<T>) comparer;
            }

            return base.GetObjectsComparer<T>(settings, parentComparer);
        }
    }
}