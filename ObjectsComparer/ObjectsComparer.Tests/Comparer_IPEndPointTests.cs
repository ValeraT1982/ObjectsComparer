using System.Linq;
using System.Net;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class Comparer_IPEndPointTests
    {
        [Test]
        public void Equality()
        {
            var a1 = new IPEndPoint(50, 20);
            var a2 = new IPEndPoint(50, 20);
            var comparer = new Comparer<IPEndPoint>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Inequality()
        {
            var a1 = new IPEndPoint(50, 20);
            var a2 = new IPEndPoint(52, 21);
            var comparer = new Comparer<IPEndPoint>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Address.Address" && d.Value1 == "50" && d.Value2 == "52"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Port" && d.Value1 == "20" && d.Value2 == "21"));
        }
    }
}