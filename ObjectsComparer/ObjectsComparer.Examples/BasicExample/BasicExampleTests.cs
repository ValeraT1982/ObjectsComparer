using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace ObjectsComparer.Examples.BasicExample
{
    [TestFixture]
    public class BasicExampleTests
    {
        [Test]
        public void EqualTest()
        {
            var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
            var a2 = new ClassA { StringProperty = "String", IntProperty = 1 };

            var comparer = new Comparer<ClassA>();
            var isEqual = comparer.Compare(a1, a2);

            Debug.WriteLine("a1 and a2 are " + (isEqual ? "equal" : "not equal"));

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NotEqualTest()
        {
            var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
            var a2 = new ClassA { StringProperty = "String", IntProperty = 2 };

            var comparer = new Comparer<ClassA>();
            IEnumerable<Difference> differenses;
            var isEqual = comparer.Compare(a1, a2, out differenses);

            var differensesList = differenses.ToList();
            Debug.WriteLine("a1 and a2 are " + (isEqual ? "equal" : "not equal"));
            if (!isEqual)
            {
                Debug.WriteLine("Differences:");
                Debug.WriteLine(string.Join(Environment.NewLine, differensesList));
            }

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differensesList.Count);
            Assert.IsTrue(differensesList.Any(d => d.MemberPath == "IntProperty" && d.Value1 == "1" && d.Value2 == "2"));
        }
    }
}
