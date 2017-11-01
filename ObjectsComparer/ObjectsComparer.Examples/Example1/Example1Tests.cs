using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static ObjectsComparer.Examples.OutputHelper;

// ReSharper disable PossibleMultipleEnumeration
namespace ObjectsComparer.Examples.Example1
{
    [TestFixture]
    public class Example1Tests
    {
        private IComparer<Message> _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new Comparer<Message>(
                new ComparisonSettings
                {
                    //Null and empty error lists are equal
                    EmptyAndNullEnumerablesEqual = true
                });
            //Do not compare Dates 
            _comparer.AddComparerOverride<DateTime>(DoNotCompareValueComparer.Instance);
            //Do not compare Id
            _comparer.AddComparerOverride(() => new Message().Id, DoNotCompareValueComparer.Instance);
            //Do not compare Message Text
            _comparer.AddComparerOverride(() => new Error().Messgae, DoNotCompareValueComparer.Instance);
        }

        [Test]
        public void EqualMessagesWithoutErrors()
        {
            var expectedMessage = new Message
            {
                MessageType = 1,
                Status = 0
            };

            var actualMessage = new Message
            {
                Id = "M12345",
                DateCreated = DateTime.Now,
                DateReceived = DateTime.Now,
                DateSent = DateTime.Now,
                MessageType = 1,
                Status = 0
            };

            IEnumerable<Difference> differences;
            var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void EqualMessagesWithErrors()
        {
            var expectedMessage = new Message
            {
                MessageType = 1,
                Status = 1,
                Errors = new List<Error>
                {
                    new Error { Id = 2 },
                    new Error { Id = 7 }
                }
            };

            var actualMessage = new Message
            {
                Id = "M12345",
                DateCreated = DateTime.Now,
                DateReceived = DateTime.Now,
                DateSent = DateTime.Now,
                MessageType = 1,
                Status = 1,
                Errors = new List<Error>
                {
                    new Error { Id = 2, Messgae = "Some error #2" },
                    new Error { Id = 7, Messgae = "Some error #7" },
                }
            };

            IEnumerable<Difference> differences;
            var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void UnequalMessages()
        {
            var expectedMessage = new Message
            {
                MessageType = 1,
                Status = 1,
                Errors = new List<Error>
                {
                    new Error { Id = 2, Messgae = "Some error #2" },
                    new Error { Id = 8, Messgae = "Some error #8" }
                }
            };

            var actualMessage = new Message
            {
                Id = "M12345",
                DateCreated = DateTime.Now,
                DateReceived = DateTime.Now,
                DateSent = DateTime.Now,
                MessageType = 1,
                Status = 2,
                Errors = new List<Error>
                {
                    new Error { Id = 2, Messgae = "Some error #2" },
                    new Error { Id = 7, Messgae = "Some error #7" }
                }
            };

            IEnumerable<Difference> differences;
            var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Status" && d.Value1 == "1" && d.Value2 == "2"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Errors[1].Id" && d.Value1 == "8" && d.Value2 == "7"));
        }
    }
}