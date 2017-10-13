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
        private class DynamicDictionary : DynamicObject
        {
            // ReSharper disable once UnusedMember.Local
            public int IntProperty { get; set; }

            private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
            
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var name = binder.Name;

                return _dictionary.TryGetValue(name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                _dictionary[binder.Name] = value;

                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _dictionary.Keys;
            }
        }


        [Test]
        public void DifferentValues()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field2 = 5;
            a1.Field3 = true;
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new DynamicDictionary();
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
        public void MissedFieldsAndUseDefaults()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new DynamicDictionary();
            a2.Field1 = "B";
            a2.Field3 = false;
            a2.Field4 = "S";
            var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "0"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Field4" && d.Value2 == "S"));
        }

        [Test]
        public void Hierarchy()
        {
            dynamic a1Sub1 = new DynamicDictionary();
            a1Sub1.Field1 = 10;
            dynamic a1 = new DynamicDictionary();
            a1.FieldSub1 = a1Sub1;
            dynamic a2Sub1 = new DynamicDictionary();
            a2Sub1.Field1 = 8;
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = null;
            dynamic a2 = new DynamicDictionary();
            a2.Field1 = null;
            var comparer = new Comparer();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullAndMissedMemberAreNotEqual()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = null;
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field2 = null;
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            dynamic a2 = new DynamicDictionary();
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
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = null;
            dynamic a2 = new DynamicDictionary();
            a2.Field1 = 5.0;
            var comparer = new Comparer();
            var doubleComparer = Substitute.For<IValueComparer>();
            doubleComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            doubleComparer.ToString(5.0).Returns("5.0");
            comparer.AddComparerOverride<double>(doubleComparer);

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == string.Empty && d.Value2 == "5.0" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        [Test]
        public void ComperaNonDynamicProperty()
        {
            dynamic a1 = new DynamicDictionary();
            a1.IntProperty = 5;
            dynamic a2 = new DynamicDictionary();
            a2.IntProperty = 7;
            var comparer = new Comparer();
            
            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "IntProperty" && d.Value1 == "5" && d.Value2 == "7" && d.DifferenceType == DifferenceTypes.ValueMismatch));
        }

        [Test]
        public void UseDefaultValuesWhenSubclassNotSpecified()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = new DynamicDictionary();
            a1.Field1.SubField1 = 0;
            a1.Field1.SubField2 = null;
            a1.Field1.SubField3 = 0.0;
            dynamic a2 = new DynamicDictionary();
            var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DifferenceWhenSubclassNotSpecified()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = new DynamicDictionary();
            a1.Field1.SubField1 = 0;
            a1.Field1.SubField2 = null;
            a1.Field1.SubField3 = 0.0;
            dynamic a2 = new DynamicDictionary();
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject));
        }
    }
}
