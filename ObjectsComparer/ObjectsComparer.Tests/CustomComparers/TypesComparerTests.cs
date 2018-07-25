using NUnit.Framework;
using System;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class TypesComparerTests
    {
        [Test]
        public void FirstParameterNotType()
        {
            var comparer = new TypesComparer(new ComparisonSettings(), null, null);
            var obj1 = 25;
            var obj2 = typeof(string);

            Assert.Throws<ArgumentException>(() => comparer.Compare(typeof(Type), obj1, obj2));
        }

        [Test]
        public void SecondParameterNotEnumerable()
        {
            var comparer = new TypesComparer(new ComparisonSettings(), null, null);
            var obj1 = typeof(string);
            var obj2 = 25;

            Assert.Throws<ArgumentException>(() => comparer.Compare(typeof(Type), obj1, obj2));
        }

        [Test]
        public void NullsAreEqual()
        {
            var comparer = new TypesComparer(new ComparisonSettings(), null, null);

            Assert.IsTrue(comparer.Compare(typeof(Type), null, null));
        }

        [Test]
        public void SameTypesAreEqual()
        {
            var comparer = new TypesComparer(new ComparisonSettings(), null, null);
            var obj1 = typeof(string);
            var obj2 = typeof(string);

            Assert.IsTrue(comparer.Compare(typeof(Type), obj1, obj2));
        }

        [Test]
        public void DifferentTypesAreNotEqual()
        {
            var comparer = new TypesComparer(new ComparisonSettings(), null, null);
            var obj1 = typeof(string);
            var obj2 = typeof(bool);

            Assert.IsFalse(comparer.Compare(typeof(Type), obj1, obj2));
        }
    }
}