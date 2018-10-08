using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static ObjectsComparer.Examples.OutputHelper;

// ReSharper disable PossibleMultipleEnumeration
namespace ObjectsComparer.Examples.Example4
{
    [TestFixture]
    public class Example4Tests
    {
        private MyComparersFactory _factory;
        private IComparer<Formula> _comparer;

        [SetUp]
        public void SetUp()
        {
            _factory = new MyComparersFactory();
            _comparer = _factory.GetObjectsComparer<Formula>();
        }

        [Test]
        public void List_Of_Equal_Sizes_But_Is_Inequality()
        {
            var formula1 = new Formula
            {
                Id = 1,
                Name = "Formula 1",
                Items = new List<FormulaItem>
                {
                    new FormulaItem
                    {
                        Id = 1,
                        Delay = 60,
                        Name = "Item 1",
                        Instruction = "Instruction 1"
                    }
                }
            };

            var formula2 = new Formula
            {
                Id = 1,
                Name = "Formula 1",
                Items = new List<FormulaItem>
                {
                    new FormulaItem
                    {
                        Id = 1,
                        Delay = 80,
                        Name = "Item One",
                        Instruction = "Instruction One"
                    }
                }
            };

            var isEqual = _comparer.Compare(formula1, formula2, out var differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Delay" && d.Value1 == "60" && d.Value2 == "80"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Name" && d.Value1 == "Item 1" && d.Value2 == "Item One"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Instruction" && d.Value1 == "Instruction 1" && d.Value2 == "Instruction One"));
        }

        [Test]
        public void List_Of_Different_Sizes_But_Is_Inequality()
        {
            var formula1 = new Formula
            {
                Id = 1,
                Name = "Formula 1",
                Items = new List<FormulaItem>
                {
                    new FormulaItem
                    {
                        Id = 1,
                        Delay = 60,
                        Name = "Item 1",
                        Instruction = "Instruction 1"
                    }
                }
            };

            var formula2 = new Formula
            {
                Id = 1,
                Name = "Formula 1",
                Items = new List<FormulaItem>
                {
                    new FormulaItem
                    {
                        Id = 1,
                        Delay = 80,
                        Name = "Item One",
                        Instruction = "Instruction One"
                    },
                    new FormulaItem
                    {
                        Id = 2,
                        Delay = 30,
                        Name = "Item Two",
                        Instruction = "Instruction Two"
                    }
                }
            };

            var isEqual = _comparer.Compare(formula1, formula2, out var differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(4, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items.Count" && d.Value1 == "1" && d.Value2 == "2"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Delay" && d.Value1 == "60" && d.Value2 == "80"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Name" && d.Value1 == "Item 1" && d.Value2 == "Item One"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Items[Id=1].Instruction" && d.Value1 == "Instruction 1" && d.Value2 == "Instruction One"));
        }
    }
}