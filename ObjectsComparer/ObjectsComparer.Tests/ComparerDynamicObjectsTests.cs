using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using NSubstitute;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerDynamicObjectsTests
    {
        [Test]
        public void DifferentValues()
        {
            dynamic a1 = new
            {
                Field1 = "A",
                Field2 = 5,
                Field3 = true
            };
            dynamic a2 = new
            {
                Field1 = "B",
                Field2 = 8,
                Field3 = false
            };

            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "true" && d.Value2 == "false"));
        }

        [Test]
        public void MissedFields()
        {
            dynamic a1 = new
            {
                Field1 = "A",
                Field2 = 5
            };
            dynamic a2 = new
            {
                Field1 = "B",
                Field2 = 8,
                Field3 = false
            };
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == string.Empty));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field3" && d.Value2 == "false"));
        }

        [Test]
        public void Hierarchy()
        {
            dynamic a1Sub1 = new { FieldSub1 = 10 };
            dynamic a1 = new { FieldSub1 = a1Sub1 };
            dynamic a2Sub1 = new { FieldSub1 = 8 };
            dynamic a2 = new { FieldSub1 = a2Sub1 };
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "FieldSub1.Field1" && d.Value1 == "10" && d.Value2 == "8"));
        }

        [Test]
        public void DifferentTypes()
        {
            dynamic a1 = new
            {
                Field1 = "A",
                Field2 = 5
            };
            dynamic a2 = new
            {
                Field1 = 5,
                Field2 = "5"
            };
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        [Test]
        public void ComparerOverrideWhenEqual()
        {
            dynamic a1 = new
            {
                Field1 = "A"
            };
            dynamic a2 = new
            {
                Field1 = " A "
            };
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), new ComparisonSettings()).Returns(true);
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            stringComparer.Received().Compare("A", " A ", new ComparisonSettings()).Returns(true);
        }

        [Test]
        public void ComparerOverrideWhenNotEqual()
        {
            dynamic a1 = new
            {
                Field1 = "A"
            };
            dynamic a2 = new
            {
                Field1 = "B"
            };
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), new ComparisonSettings()).Returns(false);
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            stringComparer.Received().Compare("A", "B", new ComparisonSettings()).Returns(false);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B" && d.DifferenceType == DifferenceTypes.ValueMismatch));
        }
    }
}
