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
            Assert.Throws<ArgumentNullException>(() => new DynamicValueComparer<string>((s1, s2) => true, null));
        }

        [Test]
        public void CompareWrongFirstArgument()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2) => true, s => s);

            Assert.Throws<ArgumentException>(() => comparer.Compare(25, "25"));
        }

        [Test]
        public void CompareWrongSecondArgument()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2) => true, s => s);

            Assert.Throws<ArgumentException>(() => comparer.Compare("25", 25));
        }

        [Test]
        public void CompareFirstArgumentNullNotValueType()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2) => s1 == s2, s => s);

            var result = comparer.Compare(null, "25");

            Assert.IsFalse(result);
        }

        [Test]
        public void CompareFirstArgumentNullValueType()
        {
            var comparer = new DynamicValueComparer<int>((i1, i2) => i1 == i2, i => i.ToString());

            Assert.Throws<ArgumentException>(() => comparer.Compare(null, 25));
        }

        [Test]
        public void CompareSecondArgumentNullNotValueType()
        {
            var comparer = new DynamicValueComparer<string>((s1, s2) => s1 == s2, s => s);

            var result = comparer.Compare("23", null);

            Assert.IsFalse(result);
        }

        [Test]
        public void CompareSecondArgumentNullValueType()
        {
            var comparer = new DynamicValueComparer<int>((i1, i2) => i1 == i2, i => i.ToString());

            Assert.Throws<ArgumentException>(() => comparer.Compare(23, null));
        }

        [TestCase("Str1", "Str2", true)]
        [TestCase("Str1", "Str1", true)]
        [TestCase("Str1", "Str", false)]
        public void Compare(string str1, string str2, bool expectedResult)
        {
            var comparer = new DynamicValueComparer<string>((s1, s2) => s1.Length == s2.Length, s => s);

            var result = comparer.Compare(str1, str2);
            var toString = comparer.ToString(str1);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(str1, toString);
        }
    }
}