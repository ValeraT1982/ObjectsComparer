using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

// ReSharper disable PossibleMultipleEnumeration

namespace ObjectsComparer.Examples.BasicExamples
{
    [TestFixture]
    public class BasicExampleTests
    {
        #region Basic
        [Test]
        public void BasicEquality()
        {
            var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
            var a2 = new ClassA { StringProperty = "String", IntProperty = 1 };
            var comparer = new Comparer<ClassA>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void BasicInequality()
        {
            var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
            var a2 = new ClassA { StringProperty = "String", IntProperty = 2 };
            var comparer = new Comparer<ClassA>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "IntProperty" && d.Value1 == "1" && d.Value2 == "2"));
        }

        [Test]
        public void BasicInequalityWhenSubclass()
        {
            var a1 = new ClassA { SubClass = new SubClassA { BoolProperty = true } };
            var a2 = new ClassA { SubClass = new SubClassA { BoolProperty = false } };
            var comparer = new Comparer<ClassA>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "SubClass.BoolProperty" && d.Value1 == "True" && d.Value2 == "False"));
        }
        #endregion

        #region Generic Enumerables
        [Test]
        public void GenericEnumerablesEquality()
        {
            var a1 = new[] { 1, 2, 3 };
            var a2 = new[] { 1, 2, 3 };
            var comparer = new Comparer<int[]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void GenericEnumerablesInequalityWhenDifferentLength()
        {
            var a1 = new[] { 1, 2 };
            var a2 = new[] { 1, 2, 3 };
            var comparer = new Comparer<int[]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Length" && d.Value1 == "2" && d.Value2 == "3"));
        }

        [Test]
        public void GenericEnumerablesInequalityWhenDifferentValue()
        {
            var a1 = new[] { 1, 2, 3 };
            var a2 = new[] { 1, 4, 3 };
            var comparer = new Comparer<int[]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "[1]" && d.Value1 == "2" && d.Value2 == "4"));
        }
        #endregion

        #region Non-Generic Enumerables
        [Test]
        public void NonGenericWhenDifferentTypes()
        {
            var a1 = new ArrayList { "Str1", "Str2" };
            var a2 = new ArrayList { "Str1", 5 };
            var comparer = new Comparer<ArrayList>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual("[1]", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.TypeMismatch, differences.First().DifferenceType);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("5", differences.First().Value2);
        }
        #endregion

        #region Multidimensional Arrays
        [Test]
        public void MultidimensionalArraysInequality()
        {
            var a1 = new[] { new[] { 1, 2 } };
            var a2 = new[] { new[] { 1, 3 } };
            var comparer = new Comparer<int[][]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count());
            Assert.AreEqual("[0][1]", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }

        [Test]
        public void MultidimensionalArraysSizeInequality1()
        {
            var a1 = new[] { new[] { 1, 2 } };
            var a2 = new[] { new[] { 2, 2 }, new[] { 3, 5 } };
            var comparer = new Comparer<int[][]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count());
            Assert.AreEqual("Length", differences.First().MemberPath);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("2", differences.First().Value2);
        }

        [Test]
        public void MultidimensionalArraysSizeInequality2()
        {
            var a1 = new[] { new[] { 1, 2 }, new[] { 3, 5 } };
            var a2 = new[] { new[] { 1, 2 }, new[] { 3, 5, 6 } };
            var comparer = new Comparer<int[][]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count());
            Assert.AreEqual("[1].Length", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }

        [Test]
        public void IntIntInequality()
        {
            var a1 = new[,] { { 1, 2 } };
            var a2 = new[,] { { 1, 3 } };
            var comparer = new Comparer<int[,]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count());
            Assert.AreEqual("[0,1]", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }

        [Test]
        public void MultidimensionalArraysSizeInequality3()
        {
            var a1 = new[,] { { 1, 2 }, { 1, 3 } };
            var a2 = new[,] { { 1, 3, 4 }, { 1, 3, 8 } };
            var comparer = new Comparer<int[,]>();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count());
            Assert.AreEqual("Dimension1", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }
        #endregion

        #region Dynamic objects (ExpandoObject)
        [Test]
        public void ExpandoObject()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            a1.Field4 = 4;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            a2.Field3 = false;
            a2.Field4 = "C";
            var comparer = new Comparer();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(4, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject && d.MemberPath == "Field2" && d.Value1 == "5"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field3" && d.Value2 == "False"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.TypeMismatch && d.MemberPath == "Field4" && d.Value1 == "4" && d.Value2 == "C"));
        }

        [Test]
        public void ExpandoObjectWhenMissedFieldsAndUseDefaults()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 0;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            a2.Field3 = false;
            a2.Field4 = "S";
            var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Field4" && d.Value2 == "S"));
        }
        #endregion

        #region Dynamic objects (DynamicObject)
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
        public void DynamicObject()
        {
            dynamic a1 = new DynamicDictionary();
            a1.Field1 = "A";
            a1.Field3 = true;
            dynamic a2 = new DynamicDictionary();
            a2.Field1 = "B";
            a2.Field2 = 8;
            a2.Field3 = 1;
            var comparer = new Comparer();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInFirstObject && d.MemberPath == "Field2" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.TypeMismatch && d.MemberPath == "Field3" && d.Value1 == "True" && d.Value2 == "1"));
        }
        #endregion

        #region Dynamic objects (compiler generated)
        [Test]
        public void CompilerGeneratedDynamicObjects()
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
                Field2 = 8.0
            };
            var comparer = new Comparer();

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare((object)a1, (object)a2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.TypeMismatch && d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == 8.0.ToString()));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedMemberInSecondObject && d.MemberPath == "Field3" && d.Value1 == "True"));
        }
        #endregion

        #region Overrides
        public class MyValueComparer : AbstractValueComparer<string>
        {
            public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
            {
                return obj1 == obj2; //Implement comparison logic here
            }
        }
        [Test]
        public void Overrides()
        {
            var comparer = new Comparer<ClassA>();

            //  Type
            //Use MyComparer to compare all members of type string 
            comparer.AddComparerOverride<string>(new MyValueComparer());
            comparer.AddComparerOverride(typeof(string), new MyValueComparer());
            //Use MyComparer to compare all members of type string except members which name starts with "Xyz"
            comparer.AddComparerOverride<string>(new MyValueComparer(), member => !member.Name.StartsWith("Xyz"));

            //  Member Info
            //Use MyValueComparer to compare StringProperty of ClassA
            comparer.AddComparerOverride(() => new ClassA().StringProperty, new MyValueComparer());
            comparer.AddComparerOverride(
                typeof(ClassA).GetTypeInfo().GetMember("StringProperty").First(),
                new MyValueComparer());
            //Compare StringProperty of ClassA by length. If length equal consider that values are equal
            comparer.AddComparerOverride(
                () => new ClassA().StringProperty,
                (s1, s2, parentSettings) => s1?.Length == s2?.Length,
                s => s.ToString());
            comparer.AddComparerOverride(
                () => new ClassA().StringProperty,
                (s1, s2, parentSettings) => s1?.Length == s2?.Length);

            //  Member Name
            comparer.AddComparerOverride("StringProperty", new MyValueComparer());


            //Exception
            var a1 = new ClassA();
            var a2 = new ClassA();
            comparer.AddComparerOverride<string>(new MyValueComparer(), member => member.Name.StartsWith("String"));
            comparer.AddComparerOverride<string>(DoNotCompareValueComparer.Instance, member => member.Name.EndsWith("Property"));

            //var result = comparer.Compare(a1, a2);//Exception here
        }
        #endregion


        private void ResultToOutput(bool isEqual, IEnumerable<Difference> differenses)
        {
            Debug.WriteLine(isEqual ? "Objects are equal" : string.Join(Environment.NewLine, differenses));
        }
    }
}
