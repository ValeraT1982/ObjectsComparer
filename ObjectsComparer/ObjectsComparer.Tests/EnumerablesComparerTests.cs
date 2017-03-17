using NUnit.Framework;
using System;
using System.Collections.Generic;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class EnumerablesComparerTests
    {
        [Test]
        public void FirstParameterNotEnumerable()
        {
            var comparer = new EnumerablesComparer(new ComparisonSettings(), null, null);
            var obj1 = 25;
            var obj2 = new List<string>();

            Assert.Throws<ArgumentException>(() => comparer.Compare(obj1, obj2));
        }

        [Test]
        public void SecondParameterNotEnumerable()
        {
            var comparer = new EnumerablesComparer(new ComparisonSettings(), null, null);
            var obj1 = new List<string>();
            var obj2 = new A();

            Assert.Throws<ArgumentException>(() => comparer.Compare(obj1, obj2));
        }
    }
}