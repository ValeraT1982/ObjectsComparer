using System.Linq;
using NUnit.Framework;
using System.Text;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class Comparer_StringBuilderTests
    {
        [Test]
        public void StringBuilderEquality()
        {
            var a1 = new StringBuilder("abc");
            var a2 = new StringBuilder("abc");
            var comparer = new Comparer<StringBuilder>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void StringBuilderInequality()
        {
            var a1 = new StringBuilder("abc");
            var a2 = new StringBuilder("abd");
            var comparer = new Comparer<StringBuilder>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(string.Empty, differences.First().MemberPath);
            Assert.AreEqual("abc", differences.First().Value1);
            Assert.AreEqual("abd", differences.First().Value2);
        }
    }
}
