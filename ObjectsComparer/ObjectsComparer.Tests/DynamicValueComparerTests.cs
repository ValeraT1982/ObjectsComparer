using System;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class DynamicValueComparerTests
    {
        [Test]
        public void ConstructorNullCompareFunc()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DynamicValueComparer<string>(null, s => s));
        }

        [Test]
        public void ConstructorNullToStringFunc()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DynamicValueComparer<string>((s1, s2, settings) => true, null));
        }

        [Test]
        public void CompareWrongFirstArgument()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => true, s => s);

            Assert.Throws<ArgumentException>(() => comparer.Compare(25, "25", new ComparisonSettings()));
        }

        [Test]
        public void CompareWrongSecondArgument()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => true, s => s);

            Assert.Throws<ArgumentException>(() => comparer.Compare("25", 25, new ComparisonSettings()));
        }

        [Test]
        public void CompareFirstArgumentNullNotValueType()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => s1 == s2, s => s);

            var result = comparer.Compare(null, "25", new ComparisonSettings());

            Assert.IsFalse(result);
        }

        [Test]
        public void CompareFirstArgumentNullValueType()
        {
            var comparer = new DynamicValueComparer<int>((i1, i2, settings) => i1 == i2, i => i.ToString());

            Assert.Throws<ArgumentException>(() => comparer.Compare(null, 25, new ComparisonSettings()));
        }

        [Test]
        public void CompareSecondArgumentNullNotValueType()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => s1 == s2, s => s);

            var result = comparer.Compare("23", null, new ComparisonSettings());

            Assert.IsFalse(result);
        }

        [Test]
        public void CompareSecondArgumentNullValueType()
        {
            var comparer = new DynamicValueComparer<int>((i1, i2, settings) => i1 == i2, i => i.ToString());

            Assert.Throws<ArgumentException>(() => comparer.Compare(23, null, new ComparisonSettings()));
        }

        [TestCase("Str1", "Str2", true)]
        [TestCase("Str1", "Str1", true)]
        [TestCase("Str1", "Str", false)]
        public void Compare(string str1, string str2, bool expectedResult)
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => s1.Length == s2.Length, s => s);

            var result = comparer.Compare(str1, str2, new ComparisonSettings());
            var toString = comparer.ToString(str1);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(str1, toString);
        }

        [TestCase("Str1", "Str2", true)]
        [TestCase("Str1", "Str1", true)]
        [TestCase("Str1", "Str", false)]
        public void CompareWithDefaultToString(string str1, string str2, bool expectedResult)
        {
            var comparer = new DynamicValueComparer<string>((s1, s2, settings) => s1.Length == s2.Length);

            var result = comparer.Compare(str1, str2, new ComparisonSettings());
            var toString = comparer.ToString(str1);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(str1, toString);
        }
    }
}