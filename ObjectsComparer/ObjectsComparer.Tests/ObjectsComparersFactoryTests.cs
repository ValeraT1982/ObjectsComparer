using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ObjectsComparersFactoryTests
    {
        [Test]
        public void GetObjectsComparerGenericTest()
        {
            var settings = new ComparisonSettings();
            var factory = new ObjectsComparersFactory();

            var comparer = factory.GetObjectsComparer<string>(settings);

            Assert.AreEqual(settings, comparer.Settings);
        }

        [Test]
        public void GetObjectsComparerTest()
        {
            var settings = new ComparisonSettings();
            var factory = new ObjectsComparersFactory();

            var comparer = factory.GetObjectsComparer(typeof(string), settings);

            Assert.AreEqual(settings, comparer.Settings);
        }
    }
}
