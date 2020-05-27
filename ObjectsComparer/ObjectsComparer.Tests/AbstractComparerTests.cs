using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class AbstractComparerTests
    {
        private BaseComparer _parentComparerMock;
        private IComparersFactory _factoryMock;
        private AbstractComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _factoryMock = Substitute.For<IComparersFactory>();
            _parentComparerMock = Substitute.ForPartsOf<BaseComparer>(new ComparisonSettings(), null, _factoryMock);
            _comparer =
                Substitute.ForPartsOf<AbstractComparer>(new ComparisonSettings(), _parentComparerMock, _factoryMock);
        }

        [Test]
        public void CalculateDifferences()
        {
            _comparer.CalculateDifferences("string1", "string2");

            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
        }

        [Test]
        public void CompareWithOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> {new Difference("", "", "string1", "string2")};
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(differences);

            var result = _comparer.Compare("string1", "string2", out var outDifferences);

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void CompareWithOutParameterWhenEqual()
        {
            var differences = new List<Difference>();
            _comparer.CalculateDifferences(typeof(string), "string1", "string1").Returns(differences);

            var result = _comparer.Compare("string1", "string1", out var outDifferences);

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string1");
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void CompareWithoutOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> {new Difference("", "", "string1", "string2")};
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(differences);

            var result = _comparer.Compare("string1", "string2");

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
        }

        [Test]
        public void CompareWithoutOutParameterWhenEqual()
        {
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(new List<Difference>());

            var result = _comparer.Compare("string1", "string2");

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
        }

        [Test]
        public void NonGenericCompareWithOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> {new Difference("", "", "string1", "string2")};
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(differences);

            var result = _comparer.Compare(typeof(string), "string1", "string2", out var outDifferences);

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void NonGenericCompareWithOutParameterWhenEqual()
        {
            var differences = new List<Difference>();
            _comparer.CalculateDifferences(typeof(string), "string1", "string1").Returns(differences);

            var result = _comparer.Compare(typeof(string), "string1", "string1", out var outDifferences);

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string1");
            Assert.AreEqual(differences, outDifferences);
        }

        [Test]
        public void NonGenericCompareWithoutOutParameterWhenNotEqual()
        {
            var differences = new List<Difference> {new Difference("", "", "string1", "string2")};
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(differences);

            var result = _comparer.Compare(typeof(string), "string1", "string2");

            Assert.IsFalse(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
        }

        [Test]
        public void NonGenericCompareWithoutOutParameterWhenEqual()
        {
            _comparer.CalculateDifferences(typeof(string), "string1", "string2").Returns(new List<Difference>());

            var result = _comparer.Compare(typeof(string), "string1", "string2");

            Assert.IsTrue(result);
            _comparer.Received().CalculateDifferences(typeof(string), "string1", "string2");
        }
    }
}