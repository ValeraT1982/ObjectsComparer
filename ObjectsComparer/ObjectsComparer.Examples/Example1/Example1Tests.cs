using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ObjectsComparer.Examples.Example1
{
    [TestFixture]
    public class Example1Tests
    {
        private IObjectsDataComparer<Message> _comparer;
        
        [SetUp]
        public void SetUp()
        {
            _comparer = new ObjectsDataComparer<Message>(new ComparisonSettings { EmptyAndNullEnumerablesEqual = true });
            _comparer.AddComparerOverride<DateTime>(DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride(() => new Message().Id, DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride(() => new Error().Messgae, DoNotCompareValueComparer.Instance);
        }

        [Test]
        public void EqualMessagesWithoutErrorsTest()
        {
            var expectedMessage = new Message
            {
                MessageType = 1,
                Status = 0,
            };

            var actualMessage = new Message
            {
                Id = "M12345",
                DateCreated = DateTime.Now,
                MessageType = 1,
                Status = 0,
            };

            var isEqual = _comparer.Compare(expectedMessage, actualMessage);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void EqualMessagesWithErrorsTest()
        {
            var expectedMessage = new Message
            {
                MessageType = 1,
                Status = 1,
                Errors = new List<Error>
                {
                    new Error { Id = 2 },
                    new Error { Id = 7 },
                }
            };

            var actualMessage = new Message
            {
                Id = "M12345",
                DateCreated = DateTime.Now,
                MessageType = 1,
                Status = 1,
                Errors = new List<Error>
                {
                    new Error { Id = 2, Messgae = "Some error #2" },
                    new Error { Id = 7, Messgae = "Some error #7" },
                }
            };

            var isEqual = _comparer.Compare(expectedMessage, actualMessage);

            Assert.IsTrue(isEqual);
        }
    }
}
