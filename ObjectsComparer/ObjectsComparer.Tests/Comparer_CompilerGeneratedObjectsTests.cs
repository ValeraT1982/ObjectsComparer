﻿using System.Linq;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerCompilerGeneratedObjectComparerObjectsTests
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

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "True" && d.Value2 == "False"));
        }

        [Test]
        public void DifferentValues_CalculateDifferenceTree()
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

            var rootNode = comparer.CalculateDifferenceTree(typeof(object), (object)a1, (object)a2);
            var differences = rootNode.GetDifferences().ToList();

            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "True" && d.Value2 == "False"));
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

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field3" && d.Value2 == "False"));
        }

        [Test]
        public void MissedFields_CheckDifferenceTreeNode()
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

            var rootDiffenenceNode = comparer.CalculateDifferenceTree(typeof(object), (object)a1, (object)a2);
            var calculateDifferences = rootDiffenenceNode.GetDifferences(true).ToArray();

            Assert.AreEqual(3, calculateDifferences.Count());
            Assert.IsTrue(calculateDifferences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(calculateDifferences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(calculateDifferences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field3" && d.Value2 == "False"));
        }

        [Test]
        public void MissedFieldsAndUseDefaults()
        {
            dynamic a1 = new
            {
                Field1 = "A",
                Field2 = 5
            };
            dynamic a2 = new
            {
                Field1 = "B",
                Field3 = false,
                Field4 = "S"
            };
            var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
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
            dynamic a1Sub1 = new { FieldSub1 = 10 };
            dynamic a1 = new { Field1 = a1Sub1 };
            dynamic a2Sub1 = new { FieldSub1 = 8 };
            dynamic a2 = new { Field1 = a2Sub1 };
            var comparer = new Comparer();

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1.FieldSub1" && d.Value1 == "10" && d.Value2 == "8"));
        }

        [Test]
        public void Hierarchy_CalculateDifferenceTree()
        {
            dynamic a1Sub1 = new { FieldSub1 = 10 };
            dynamic a1 = new { Field1 = a1Sub1 };
            dynamic a2Sub1 = new { FieldSub1 = 8 };
            dynamic a2 = new { Field1 = a2Sub1 };
            var comparer = new Comparer();

            var rootNode = comparer.CalculateDifferenceTree(typeof(object), (object)a1, (object)a2);
            var differences = rootNode.GetDifferences().ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1.FieldSub1" && d.Value1 == "10" && d.Value2 == "8"));

            var field1 = rootNode.Descendants.First();
            var fieldSub1 = field1.Descendants.First();

            Assert.IsTrue(differences.Any(d => d.MemberPath == $"{field1.Member.Name}.{fieldSub1.Member.Name}" && d.Value1 == "10" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == $"{field1.Member.Info.Name}.{fieldSub1.Member.Info.Name}" && d.Value1 == "10" && d.Value2 == "8"));
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

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        [Test]
        public void NullsAreEqual()
        {
            dynamic a1 = new
            {
                Field1 = (object)null
            };
            dynamic a2 = new
            {
                Field1 = (object)null
            };
            var comparer = new Comparer();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullAndMissedMemberAreNotEqual()
        {
            dynamic a1 = new
            {
                Field1 = (object)null
            };
            dynamic a2 = new
            {
                Field2 = (object)null
            };
            var comparer = new Comparer();

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject));
        }

        [Test]
        public void NullAndMissedMemberAreNotEqual_CheckDifferenceTreeNode()
        {
            dynamic a1 = new
            {
                Field1 = (object)null
            };
            dynamic a2 = new
            {
                Field2 = (object)null
            };

            var t = (a1 as object).GetType();
            var members = t.GetMembers();

            var comparer = new Comparer();
            

            var rootDifferenceNode = comparer.CalculateDifferenceTree(typeof(object), (object)a1, (object)a2);
            var calculateDifferences = rootDifferenceNode.GetDifferences(true);

            Assert.IsTrue(calculateDifferences.Any());
            Assert.AreEqual(2, calculateDifferences.Count());
            Assert.IsTrue(calculateDifferences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject));
            Assert.IsTrue(calculateDifferences.Any(
                d => d.MemberPath == "Field2" && d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject));
        }

        [Test]
        public void NullValues()
        {
            dynamic a1 = new
            {
                Field1 = "A",
                Field2 = (object)null
            };
            dynamic a2 = new
            {
                Field1 = (object)null,
                Field2 = "B"
            };
            var comparer = new Comparer();

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
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
            dynamic a1 = new
            {
                Field1 = "A"
            };
            dynamic a2 = new
            {
                Field1 = " A "
            };
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            var isEqual = comparer.Compare((object)a1, (object)a2);

            Assert.IsTrue(isEqual);
            stringComparer.Received().Compare("A", " A ", Arg.Any<ComparisonSettings>());
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
            stringComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<object>()).Returns(callInfo => callInfo.Arg<object>()?.ToString());
            var comparer = new Comparer();
            comparer.AddComparerOverride("Field1", stringComparer);

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
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
            dynamic a1 = new
            {
                Field1 = (object)null
            };
            dynamic a2 = new
            {
                Field1 = 5
            };
            var comparer = new Comparer();
            var intComparer = Substitute.For<IValueComparer>();
            intComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            intComparer.ToString(5).Returns("5");
            comparer.AddComparerOverride<int>(intComparer);

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == string.Empty && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        [Test]
        public void UseDefaultValuesWhenSubclassNotSpecified()
        {
            dynamic a1 = new
            {
                Field1 = new
                {
                    SubField1 = 0,
                    SubField2 = (object) null,
                    SubField3 = 0.0
                }
            };
            dynamic a2 = new {};
            var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });

            var isEqual = comparer.Compare((object)a1, (object)a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DifferenceWhenSubclassNotSpecified()
        {
            dynamic a1 = new
            {
                Field1 = new
                {
                    SubField1 = 0,
                    SubField2 = (object)null,
                    SubField3 = 0.0
                }
            };
            dynamic a2 = new { };
            var comparer = new Comparer();

            var isEqual = comparer.Compare((object)a1, (object)a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject));
        }
    }
}
