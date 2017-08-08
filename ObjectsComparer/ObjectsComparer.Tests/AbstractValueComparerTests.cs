using NSubstitute;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class AbstractValueComparerTests
    {
        [Test]
        public void DefaultToString()
        {
            var valueComparer = Substitute.ForPartsOf<AbstractValueComparer<string>>();

            var result = valueComparer.ToString("str1");

            Assert.AreEqual("str1", result);
        }
    }
}
