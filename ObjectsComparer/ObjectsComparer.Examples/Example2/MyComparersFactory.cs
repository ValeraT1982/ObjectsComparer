using System;

namespace ObjectsComparer.Examples.Example2
{
    public class MyComparersFactory: ComparersFactory
    {
        public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, IBaseComparer parentComparer = null)
        {
            if (typeof(T) == typeof(Person))
            {
                var comparer = new Comparer<Person>(settings, parentComparer, this);
                comparer.AddComparerOverride<Guid>(DoNotCompareValueComparer.Instance);
                comparer.AddComparerOverride(
                    () => new Person().MiddleName,
                    new DynamicValueComparer<string>(
                        (s1, s2, parentSettings) => string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2) || s1 == s2,
                        s => s));
                comparer.AddComparerOverride(
                    () => new Person().PhoneNumber,
                    new PhoneNumberComparer());

                return (IComparer<T>)comparer;
            }

            return base.GetObjectsComparer<T>(settings, parentComparer);
        }
    }
}
