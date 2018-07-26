using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class AbstractComparerGenericTests
    {
        private BaseComparer _parentComparerMock;
        private IComparersFactory _factoryMock;
        private AbstractComparer<int> _comparer;

        [SetUp]
        public void SetUp()
        {
            _factoryMock = Substitute.For<IComparersFactory>();
            _parentComparerMock = Substitute.ForPartsOf<BaseComparer>(new ComparisonSettings(), null, _factoryMock);
            _comparer =
                Substitute.ForPartsOf<AbstractComparer<int>>(new ComparisonSettings(), _parentComparerMock, _factoryMock);
        }

        [Test]
        public void CalculateDifferences()
        {
            _comparer.CalculateDifferences(1, 2);

            _comparer.Received().CalculateDifferences(1, 2);
        }

        [Test]
        public void CompareWithOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> { new Difference("", "1", "2") };
            _comparer.CalculateDifferences(1, 2).Returns(differences);

            var result = _comparer.Compare(1, 2, out var outDifferences);

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(1, 2);
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void CompareWithOutParameterWhenEqual()
        {
            var differences = new List<Difference>();
            _comparer.CalculateDifferences(1, 1).Returns(differences);

            var result = _comparer.Compare(1, 1, out var outDifferences);

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(1, 1);
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void CompareWithoutOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> { new Difference("", "1", "2") };
            _comparer.CalculateDifferences(1, 2).Returns(differences);

            var result = _comparer.Compare(1, 2);

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(1, 2);
        }

        [Test]
        public void CompareWithoutOutParameterWhenEqual()
        {
            _comparer.CalculateDifferences(1, 2).Returns(new List<Difference>());

            var result = _comparer.Compare(1, 2);

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(1, 2);
        }
    }
}
