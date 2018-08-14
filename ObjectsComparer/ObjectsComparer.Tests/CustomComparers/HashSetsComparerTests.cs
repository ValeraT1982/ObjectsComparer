using NUnit.Framework;
using System;
using System.Collections.Generic;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class HashSetsComparerTests
    {
        [Test]
        public void FirstParameterNotEnumerable()
        {
            var comparer = new HashSetsComparer<string>(new ComparisonSettings(), null, null);
            var obj1 = 25;
            var obj2 = new HashSet<string>();

            Assert.Throws<ArgumentException>(() => comparer.Compare(typeof(HashSet<string>), obj1, obj2));
        }

        [Test]
        public void SecondParameterNotEnumerable()
        {
            var comparer = new HashSetsComparer<string>(new ComparisonSettings(), null, null);
            var obj1 = new HashSet<string>();
            var obj2 = new A();

            Assert.Throws<ArgumentException>(() => comparer.Compare(typeof(HashSet<string>), obj1, obj2));
        }

        [Test]
        public void TypeNotHashSet()
        {
            var comparer = new HashSetsComparer<string>(new ComparisonSettings(), null, null);
            var obj1 = new HashSet<string>();
            var obj2 = new HashSet<string>();

            Assert.Throws<ArgumentException>(() => comparer.Compare(typeof(int), obj1, obj2));
        }
    }
}