using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class Comparer_Issue24Tests
    {
        class MyComparersFactory : ComparersFactory
        {
            public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null,
                BaseComparer parentComparer = null)
            {

                IComparer<T> comparer = base.GetObjectsComparer<T>(settings, parentComparer);

                if (parentComparer == null)
                {
                    comparer.AddComparerOverride(typeof(string), new IgnoreCaseStringsValueComparer());
                }

                return comparer;
            }
        }

        class TestClassA
        {
            public string A { get; set; }
        }

        class TestClassB
        {
            public string B { get; set; }

            public TestClassA ClassA { get; set; }
        }

        [Test]
        public void OverrideString()
        {
            var b1 = new TestClassB
            {
                B = "B",
                ClassA = new TestClassA
                {
                    A = "A"
                }
            };

            var b2 = new TestClassB
            {
                B = "b",
                ClassA = new TestClassA
                {
                    A = "a"
                }
            };

            var settings = new ComparisonSettings();
            var factory = new MyComparersFactory();

            var comparer = factory.GetObjectsComparer<TestClassB>(settings);

            var isEqual = comparer.Compare(b1, b2);

            Assert.IsTrue(isEqual);
        }
    }
}