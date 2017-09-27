using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using NSubstitute;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerExpandoObjectsTests
    {
        [Test]
        public void DifferentValues()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            a1.Field3 = true;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            a2.Field2 = 8;
            a2.Field3 = false;

            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "True" && d.Value2 == "False"));
        }

        [Test]
        public void MissedFields()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            a2.Field3 = false;
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject && d.MemberPath == "Field2" && d.Value1 == "5"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field3" && d.Value2 == "False"));
        }

        [Test]
        public void Hierarchy()
        {
            dynamic a1Sub1 = new ExpandoObject();
            a1Sub1.Field1 = 10;
            dynamic a1 = new ExpandoObject();
            a1.FieldSub1 = a1Sub1;
            dynamic a2Sub1 = new ExpandoObject();
            a2Sub1.Field1 = 8;
            dynamic a2 = new ExpandoObject();
            a2.FieldSub1 = a2Sub1;
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
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = 5;
            a2.Field2 = "5";
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.TypeMismatch));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        [Test]
        public void NullsAreEqual()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = null;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = null;
            var comparer = new Comparer();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullAndMissedMemberAreNotEqual()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = null;
            dynamic a2 = new ExpandoObject();
            a2.Field2 = null;
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject));
        }

        [Test]
        public void NullValues()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = null;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = null;
            a2.Field2 = "B";
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.DifferenceType == DifferenceTypes.ValueMismatch));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.Value2 == "B" && d.DifferenceType == DifferenceTypes.ValueMismatch));
        }

        [Test]
        public void ComparerOverrideWhenEqual()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            dynamic a2 = new ExpandoObject();
            a2.Field1 = " A ";
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            stringComparer.Received().Compare("A", " A ", Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void ComparerOverrideWhenNotEqual()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            stringComparer.Received().Compare("A", "B", Arg.Any<ComparisonSettings>());
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B" && d.DifferenceType == DifferenceTypes.ValueMismatch));
        }

        [Test]
        public void ComparerOverrideWhenNullAndValueType()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = null;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = 5;
            var comparer = new Comparer();
            var intComparer = Substitute.For<IValueComparer>();
            intComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            comparer.AddComparerOverride<int>(intComparer);

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == null && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }
    }
}
