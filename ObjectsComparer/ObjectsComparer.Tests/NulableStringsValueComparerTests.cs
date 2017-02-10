using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class NulableStringsValueComparerTests
    {
        [TestCase(null, null, true)]
        [TestCase(null, "", true)]
        [TestCase("", null, true)]
        [TestCase("", "", true)]
        [TestCase("str", "str", true)]
        [TestCase(null, "s", false)]
        [TestCase("hhh", null, false)]
        [TestCase("hhh", "s", false)]
        public void Compare(string s1, string s2, bool expectedResult)
        {
            var comparer = new NulableStringsValueComparer();

            var result = comparer.Compare(s1, s2);

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("ss", "ss")]
        public void ToString(string s, string expectedToString)
        {
            var comparer = new NulableStringsValueComparer();

            var result = comparer.ToString(s);

            Assert.AreEqual(expectedToString, result);
        }
    }
}