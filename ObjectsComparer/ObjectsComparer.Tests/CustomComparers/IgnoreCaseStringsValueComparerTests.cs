using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class IgnoreCaseStringsValueComparerTests
    {
        [TestCase(null, null, true)]
        [TestCase("", "", true)]
        [TestCase("str", "str", true)]
        [TestCase(null, "s", false)]
        [TestCase("hhh", null, false)]
        [TestCase("hhh", "s", false)]
        [TestCase("hhh", "HHH", true)]
        [TestCase("hhh", "HHH", true)]
        [TestCase("Go", "go", true)]
        public void Compare(string s1, string s2, bool expectedResult)
        {
            var comparer = new IgnoreCaseStringsValueComparer();

            var result = comparer.Compare(s1, s2, new ComparisonSettings());

            Assert.AreEqual(expectedResult, result);
        }
    }
}