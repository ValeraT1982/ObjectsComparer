using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class DoNotCompareValueComparerTests
    {
        [Test]
        public void Instance()
        {
            Assert.IsNotNull(DoNotCompareValueComparer.Instance);
        }

        [Test]
        public void CompareTest()
        {
            var result = DoNotCompareValueComparer.Instance.Compare(25, "String");

            Assert.IsTrue(result);
        }

        [TestCase(25)]
        [TestCase("String")]
        [TestCase(12.5)]
        [TestCase(null)]
        public void ToStringTest(object value)
        {
            var result = DoNotCompareValueComparer.Instance.ToString(value);

            Assert.AreEqual(string.Empty, result);
        }
    }
}