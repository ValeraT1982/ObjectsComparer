using NUnit.Framework;

namespace ObjectsComparer.Examples.Example5
{
    [TestFixture]
    public class Example5Tests
    {
        [Test]
        public void IgnoreByAttribute()
        {
            var error1 = new Error
            {
                Id = 1,
                Messgae = "Error Message",
                Details = "Error details"
            };

            var error2 = new Error
            {
                Id = 1,
                Messgae = "Error Message",
                Details = "Other error details"
            };

            var comparer = new Comparer<Error>();
            comparer.AddComparerOverride<string>(DoNotCompareValueComparer.Instance, m => m.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0);

            var isEqual = comparer.Compare(error1, error2);

            Assert.IsTrue(isEqual);
        }
    }
}