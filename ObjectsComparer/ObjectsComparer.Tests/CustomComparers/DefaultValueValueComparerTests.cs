using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class DefaultValueValueComparerTests
    {
        [TestCase(0, 5, true)]
        [TestCase(null, 5, true)]
        [TestCase(5, 0, true)]
        [TestCase(5, null, true)]
        [TestCase(5, 0, true)]
        [TestCase(0, 5, true)]
        [TestCase(0, 0, true)]
        [TestCase(5, 5, true)]
        [TestCase(null, 7, false)]
        [TestCase(0, 7, false)]
        [TestCase(5, 7, false)]
        [TestCase(7, null, false)]
        [TestCase(7, 0, false)]
        [TestCase(7, 5, false)]
        [TestCase(7, 8, false)]
        public void ValueType(object obj1, object obj2, bool expectedResult)
        {
            var comparer = new DefaultValueValueComparer<int>(5, new DefaultValueComparer());

            var actualResult = comparer.Compare(obj1, obj2, new ComparisonSettings());

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(null, "none", true)]
        [TestCase("none", null, true)]
        [TestCase(null, null, true)]
        [TestCase("none", "none", true)]
        [TestCase(null, "string", false)]
        [TestCase("string", null, false)]
        [TestCase("none", "string", false)]
        [TestCase("string", "none", false)]
        [TestCase("string1", "string2", false)]
        public void ReferenceType(object obj1, object obj2, bool expectedResult)
        {
            var comparer = new DefaultValueValueComparer<string>("none", new DefaultValueComparer());

            var actualResult = comparer.Compare(obj1, obj2, new ComparisonSettings());

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
