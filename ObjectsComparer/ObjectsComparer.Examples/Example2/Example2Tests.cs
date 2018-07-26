using System;
using System.Linq;
using NUnit.Framework;
using static ObjectsComparer.Examples.OutputHelper;

// ReSharper disable PossibleMultipleEnumeration
namespace ObjectsComparer.Examples.Example2
{
    [TestFixture]
    public class Example2Tests
    {
        private MyComparersFactory _factory;
        private IComparer<Person> _comparer;

        [SetUp]
        public void SetUp()
        {
            _factory = new MyComparersFactory();
            _comparer = _factory.GetObjectsComparer<Person>();
        }

        [Test]
        public void EqualPersonsTest()
        {
            var person1 = new Person
            {
                PersonId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "F",
                PhoneNumber = "111-555-8888"
            };
            var person2 = new Person
            {
                PersonId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "(111) 555 8888"
            };

            var isEqual = _comparer.Compare(person1, person2, out var differences);

            ResultToOutput(isEqual, differences);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DifferentPersonsTest()
        {
            var person1 = new Person
            {
                PersonId = Guid.NewGuid(),
                FirstName = "Jack",
                LastName = "Doe",
                MiddleName = "F",
                PhoneNumber = "111-555-8888"
            };
            var person2 = new Person
            {
                PersonId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "L",
                PhoneNumber = "222-555-9999"
            };

            var isEqual = _comparer.Compare(person1, person2, out var differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "FirstName" && d.Value1 == "Jack" && d.Value2 == "John"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "MiddleName" && d.Value1 == "F" && d.Value2 == "L"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "PhoneNumber" && d.Value1 == "111-555-8888" && d.Value2 == "222-555-9999"));
        }
    }
}
