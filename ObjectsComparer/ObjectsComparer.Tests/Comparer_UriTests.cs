using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class Comparer_UriTests
    {
        public class TestClass
        {
            public IList<Uri> Urls { get; } = new List<Uri>();
        }

        [Test]
        public void Equality()
        {
            var a1 = new { MyUri = new Uri("https://www.google.com/") };
            var a2 = new { MyUri = new Uri("https://www.google.com/") };
            var comparer = new Comparer();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Inequality()
        {
            var a1 = new { MyUri = new Uri("https://www.google.com/") };
            var a2 = new { MyUri = new Uri("https://www.yahoo.com/") };
            var comparer = new Comparer();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("MyUri", differences.First().MemberPath);
        }

        [Test]
        public void ListOfUris()
        {
            var a1 = new TestClass();
            a1.Urls.Add(new Uri("https://Test.com"));
            var a2 = new TestClass();
            a2.Urls.Add(new Uri("https://Test.com"));

            var comparer = new ObjectsComparer.Comparer<TestClass>();
            //  comparer.AddComparerOverride<IList<Uri>>(new ListOfUriComparer());
            IEnumerable<Difference> differences = new List<Difference>();
            comparer.Compare(a1, a2, out differences);
        }
    }
}