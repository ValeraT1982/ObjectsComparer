using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class DefaultValueComparerTests
    {
        [TestCase(null, null, true)]
        [TestCase(5, null, false)]
        [TestCase(null, "string", false)]
        [TestCase(5, "string", false)]
        [TestCase(5, 5, true)]
        [TestCase(5, 7, false)]
        [TestCase("string1", "string1", true)]
        [TestCase("string1", "string2", false)]
        public void Compare(object obj1, object obj2, bool expectedResult)
        {
            var comparer = new DefaultValueComparer();

            var actualResult = comparer.Compare(obj1, obj2, new ComparisonSettings());

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(null, "")]
        [TestCase(5, "5")]
        [TestCase("str1", "str1")]
        [TestCase(true, "True")]
        public void Compare(object value, string expectedResult)
        {
            var comparer = new DefaultValueComparer();

            var actualResult = comparer.ToString(value);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}