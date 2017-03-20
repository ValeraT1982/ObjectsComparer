using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparersFactoryTests
    {
        private IComparer<B> _comparerB;

        private class CustomFactory : ComparersFactory
        {
            private readonly IComparer<B> _comparerB;

            public CustomFactory(IComparer<B> comparerB)
            {
                _comparerB = comparerB;
            }

            public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, IBaseComparer parentComparer = null)
            {
                if (typeof(T) == typeof(B))
                {
                    return (IComparer<T>)_comparerB;
                }

                return base.GetObjectsComparer<T>(settings, parentComparer);
            }
        }

        [SetUp]
        public void SetUp()
        {
            _comparerB = Substitute.For<IComparer<B>>();
        }

        [Test]
        public void GetObjectsComparerGenericTest()
        {
            var settings = new ComparisonSettings();
            var factory = new ComparersFactory();

            var comparer = factory.GetObjectsComparer<string>(settings);

            Assert.AreEqual(settings, comparer.Settings);
        }

        [Test]
        public void GetObjectsComparerTest()
        {
            var settings = new ComparisonSettings();
            var factory = new ComparersFactory();

            var comparer = factory.GetObjectsComparer(typeof(string), settings);

            Assert.AreEqual(settings, comparer.Settings);
        }

        [Test]
        public void CustomFactoryGenericMethodTest()
        {
            var factory = new CustomFactory(_comparerB);

            var comparer = factory.GetObjectsComparer<B>();

            Assert.AreEqual(_comparerB, comparer);
        }

        [Test]
        public void CustomFactoryNongenericMethodTest()
        {
            var factory = new CustomFactory(_comparerB);
            var comparer = factory.GetObjectsComparer(typeof(B));
            var b1 = new B();
            var b2 = new B();
            _comparerB.CalculateDifferences(b1, b2).Returns(new List<Difference>());

            var isEqual = comparer.Compare(typeof(B), b1, b2);

            Assert.IsTrue(isEqual);
            _comparerB.Received().CalculateDifferences(b1, b2);
        }
    }
}
