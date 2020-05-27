using NUnit.Framework;
using System.Collections.Generic;
using static ObjectsComparer.Examples.OutputHelper;

namespace ObjectsComparer.Examples.Example7
{
    [TestFixture]
    public class Example7Tests
    {
        private IComparer<Element> _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new Comparer<Element>();
        }

        [Test]
        public void EqualPersonsTest()
        {
            var e1 = new Element
            {
                Name = "N1",
                Items = new List<ElementItem>() {
                    new ElementItem() { ElementId = 101, Description = "D10", Price = new ElementPrice() { Value = 1.1m } },
                    new ElementItem() { ElementId = 102, Description = "D20", Price = new ElementPrice() { Value = 2.2m } }
                }
            };

            var e2 = new Element
            {
                Name = "N2",
                Items = new List<ElementItem>() {
                    new ElementItem() { ElementId = 101, Description = "D11", Price = new ElementPrice() { Value = 2.2m } },
                    new ElementItem() { ElementId = 103, Description = "D30", Price = new ElementPrice() { Value = 3.3m } },
                    new ElementItem() { ElementId = 104, Description = "D30", Price = new ElementPrice() { Value = 4.4m } },
                }
            };

            var isEqual = _comparer.Compare(e1, e2, out var differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
        }
    }
}
