using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ObjectsComparer.Examples.Example2
{
    [TestFixture]
    public class Example2Tests
    {
        private MyObjectsComparersFactory _factory;
        private IObjectsDataComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _factory = new MyObjectsComparersFactory();
            _comparer = _factory.GetObjectsComparer<Customer>();
        }

        [Test]
        public void EqualCustomersTest()
        {
            var customer1 = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "F",
                PhoneNumber = "111-555-8888"
            };
            var customer2 = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "(111) 555 8888"
            };


            IEnumerable<Difference> differenses;
            var isEqual = _comparer.Compare(customer1, customer2, out differenses);

            Assert.IsTrue(isEqual);
        }
    }
}
