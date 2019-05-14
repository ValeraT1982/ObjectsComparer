using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ParentInterfacePropertiesTests
    {
        interface IParent
        {
            string Property1 { get; set; }
        }

        interface IChild : IParent
        {
            string Property2 { get; set; }
        }

        class Child : IChild
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [Test]
        public void ComparePropertyOfParentInterface()
        {
            var a1 = new Child {Property1 = "str11", Property2 = "str12"};
            var a2 = new Child {Property1 = "str21", Property2 = "str22"};

            var comparer = new Comparer<IChild>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);
            Assert.AreEqual("Property1", differences[1].MemberPath);
            Assert.AreEqual("str11", differences[1].Value1);
            Assert.AreEqual("str21", differences[1].Value2);
            Assert.AreEqual("Property2", differences[0].MemberPath);
            Assert.AreEqual("str12", differences[0].Value1);
            Assert.AreEqual("str22", differences[0].Value2);
        }
    }
}