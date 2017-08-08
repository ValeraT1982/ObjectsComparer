using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using System.Collections.Generic;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerTests
    {
        [Test]
        public void PropertyEquality()
        {
            var a1 = new A { IntProperty = 10, DateTimeProperty = new DateTime(2017, 1, 1), Property3 = 5 };
            var a2 = new A { IntProperty = 10, DateTimeProperty = new DateTime(2017, 1, 1), Property3 = 8 };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void PropertyInequality()
        {
            var date1 = new DateTime(2017, 1, 1);
            var date2 = new DateTime(2017, 1, 2);
            var a1 = new A { IntProperty = 10, DateTimeProperty = date1 };
            var a2 = new A { IntProperty = 8, DateTimeProperty = date2 };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntProperty", differences[0].MemberPath);
            Assert.AreEqual("10", differences[0].Value1);
            Assert.AreEqual("8", differences[0].Value2);
            Assert.AreEqual("DateTimeProperty", differences[1].MemberPath);
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            Assert.AreEqual(date1.ToString(), differences[1].Value1);
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            Assert.AreEqual(date2.ToString(), differences[1].Value2);
        }

        [Test]
        public void ReadOnlyPropertyEquality()
        {
            var a1 = new A(1.99);
            var a2 = new A(1.99);
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ReadOnlyPropertyInequality()
        {
            var a1 = new A(1.99);
            var a2 = new A(0.89);
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ReadOnlyProperty", differences.First().MemberPath);
            Assert.AreEqual("1.99", differences.First().Value1);
            Assert.AreEqual("0.89", differences.First().Value2);
        }

        [Test]
        public void ProtectedProperty()
        {
            var a1 = new A(true);
            var a2 = new A(false);
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void FieldEquality()
        {
            var a1 = new A { Field = 9 };
            var a2 = new A { Field = 9 };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void FieldInequality()
        {
            var a1 = new A { Field = 10 };
            var a2 = new A { Field = 8 };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("Field", differences.First().MemberPath);
            Assert.AreEqual("10", differences.First().Value1);
            Assert.AreEqual("8", differences.First().Value2);
        }

        [Test]
        public void ReadOnlyFieldEquality()
        {
            var a1 = new A("Str1");
            var a2 = new A("Str1");
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ReadOnlyFieldInequality()
        {
            var a1 = new A("Str1");
            var a2 = new A("Str2");
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ReadOnlyField", differences.First().MemberPath);
            Assert.AreEqual("Str1", differences.First().Value1);
            Assert.AreEqual("Str2", differences.First().Value2);
        }

        [Test]
        public void ProtectedField()
        {
            var a1 = new A(5);
            var a2 = new A(6);
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ClassPropertyEquality()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { ClassB = new B { Property1 = "Str1" } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ClassPropertyInequality()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { ClassB = new B { Property1 = "Str2" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB.Property1", differences.First().MemberPath);
            Assert.AreEqual("Str1", differences.First().Value1);
            Assert.AreEqual("Str2", differences.First().Value2);
        }

        [Test]
        public void ClassPropertyInequalityFirstNull()
        {
            var a1 = new A();
            var a2 = new A { ClassB = new B { Property1 = "Str2" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB", differences.First().MemberPath);
            Assert.AreEqual("", differences.First().Value1);
            Assert.AreEqual(a2.ClassB.ToString(), differences.First().Value2);
        }

        [Test]
        public void ClassPropertyInequalitySecondNull()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str2" } };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB", differences.First().MemberPath);
            Assert.AreEqual(a1.ClassB.ToString(), differences.First().Value1);
            Assert.AreEqual("", differences.First().Value2);
        }

        [Test]
        public void NoRecursiveComparison()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { ClassB = new B { Property1 = "Str2" } };
            var comparer = new Comparer<A>(new ComparisonSettings { RecursiveComparison = false });

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ValueTypeArrayEquality()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 2 } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void PrimitiveTypeArrayInequalityCount()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 2, 3 } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("IntArray.Length", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("3", differences[0].Value2);
        }

        [Test]
        public void PrimitiveTypeArrayInequalityMember()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntArray[1]", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }

        [Test]
        public void PrimitiveTypeArrayInequalityFirstNullNull()
        {
            var a1 = new A();
            var a2 = new A { IntArray = new int[0] };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntArray", differences.First().MemberPath);
            Assert.AreEqual(string.Empty, differences.First().Value1);
            Assert.AreEqual(a2.IntArray.ToString(), differences.First().Value2);
        }

        [Test]
        public void PrimitiveTypeArrayInequalitySecondNullNull()
        {
            var a1 = new A { IntArray = new int[0] };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntArray", differences.First().MemberPath);
            Assert.AreEqual(a1.IntArray.ToString(), differences.First().Value1);
            Assert.AreEqual(string.Empty, differences.First().Value2);
        }

        [Test]
        public void ClassArrayEquality()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ClassArrayInequalityCount()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("ArrayOfB.Length", differences[0].MemberPath);
            Assert.AreEqual("1", differences[0].Value1);
            Assert.AreEqual("2", differences[0].Value2);
        }

        [Test]
        public void ClassArrayInequalityProperty()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str3" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ArrayOfB[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void CollectionEquality()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void CollectionInequalityCount()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("CollectionOfB.Count", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);
        }

        [Test]
        public void CollectionInequalityProperty()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str3" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("CollectionOfB[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void ClassImplementsCollectionEquality()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ClassImplementsCollectionInequalityCount()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("ClassImplementsCollectionOfB.Count", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);
        }

        [Test]
        public void ClassImplementsCollectionInequalityProperty()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str3" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassImplementsCollectionOfB[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void InterfacePropertyEquality()
        {
            var a1 = new A { IntefaceProperty = new TestInterfaceImplementation1 { Property = "Str1" } };
            var a2 = new A { IntefaceProperty = new TestInterfaceImplementation2 { Property = "Str1", AnotherProperty = 50 } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void InterfacePropertyInequality()
        {
            var a1 = new A { IntefaceProperty = new TestInterfaceImplementation1 { Property = "Str1" } };
            var a2 = new A { IntefaceProperty = new TestInterfaceImplementation2 { Property = "Str2", AnotherProperty = 50 } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntefaceProperty.Property", differences.First().MemberPath);
            Assert.AreEqual("Str1", differences.First().Value1);
            Assert.AreEqual("Str2", differences.First().Value2);
        }

        [Test]
        public void StructPropertyEquality()
        {
            var a1 = new A { StructProperty = new TestStruct { FieldA = "FA", FieldB = "FB" } };
            var a2 = new A { StructProperty = new TestStruct { FieldA = "FA", FieldB = "FB" } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void StructPropertyInequality()
        {
            var a1 = new A { StructProperty = new TestStruct { FieldA = "FA", FieldB = "FB" } };
            var a2 = new A { StructProperty = new TestStruct { FieldA = "FA", FieldB = "FBB" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("StructProperty.FieldB", differences.First().MemberPath);
            Assert.AreEqual("FB", differences.First().Value1);
            Assert.AreEqual("FBB", differences.First().Value2);
        }

        [Test]
        public void EnumPropertyEquality()
        {
            var a1 = new A { EnumProperty = TestEnum.Value1 };
            var a2 = new A { EnumProperty = TestEnum.Value1 };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void EnumPropertyInequality()
        {
            var a1 = new A { EnumProperty = TestEnum.Value1 };
            var a2 = new A { EnumProperty = TestEnum.Value2 };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("EnumProperty", differences.First().MemberPath);
            Assert.AreEqual("Value1", differences.First().Value1);
            Assert.AreEqual("Value2", differences.First().Value2);
        }

        [Test]
        public void SetDefaultComparer()
        {
            var a1 = new A { TestProperty1 = "ABC", Field = 5 };
            var a2 = new A { TestProperty1 = "BCD", Field = 6 };
            var comparer = new Comparer<A>();
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.SetDefaultComparer(valueComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            valueComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void SetDefaultComparerNullException()
        {
            var comparer = new Comparer<A>();

            Assert.Throws<ArgumentNullException>(() => comparer.SetDefaultComparer(null));
        }


        [Test]
        public void NonGenericEnumerableEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NonGenericEnumerableInequalityCount()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[]", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
        }

        [Test]
        public void NonGenericEnumerableInequalityProperty()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str3" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void NonGenericEnumerableInequalityType()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, "Str3" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[1]", differences.First().MemberPath);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void NonGenericEnumerableImplementationEquality()
        {
            var a1 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var a2 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NonGenericEnumerableImplementationInequalityProperty()
        {
            var a1 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) { Property1 = "Str3" } };
            var a2 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) { Property1 = "Str4" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerableImplementation.Property1", differences.First().MemberPath);
            Assert.AreEqual("Str3", differences.First().Value1);
            Assert.AreEqual("Str4", differences.First().Value2);
        }

        [Test]
        public void NullAndEmptyComparisonNonGenericInequality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList() };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[]", differences.First().MemberPath);
            Assert.AreEqual("System.Collections.ArrayList", differences.First().Value1);
            Assert.AreEqual(string.Empty, differences.First().Value2);
        }

        [Test]
        public void NullAndEmptyComparisonNonGenericEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList() };
            var a2 = new A();
            var comparer = new Comparer<A>(new ComparisonSettings { EmptyAndNullEnumerablesEqual = true });

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullAndEmptyComparisonGenericInequality()
        {
            var a1 = new A { ListOfB = new List<B>() };
            var a2 = new A();
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ListOfB", differences.First().MemberPath);
            Assert.AreEqual(a1.ListOfB.ToString(), differences.First().Value1);
            Assert.AreEqual(string.Empty, differences.First().Value2);
        }

        [Test]
        public void NullAndEmptyComparisonGenericEquality()
        {
            var a1 = new A { ListOfB = new List<B>() };
            var a2 = new A();
            var comparer = new Comparer<A>(new ComparisonSettings { EmptyAndNullEnumerablesEqual = true });

            IEnumerable<Difference> differences;
            var isEqual = comparer.Compare(a1, a2, out differences);

            Assert.IsTrue(isEqual);
            CollectionAssert.IsEmpty(differences);
        }

        [Test]
        public void InheritedAndBaseClassInequality()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { ClassB = new InheritedFromB { Property1 = "Str2", NewProperty = "SomeValue" } };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("ClassB.Property1", differences.First().MemberPath);
            Assert.AreEqual("Str1", differences.First().Value1);
            Assert.AreEqual("Str2", differences.First().Value2);
        }

        [Test]
        public void InheritedAndBaseClassEquality()
        {
            var a1 = new A { ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { ClassB = new InheritedFromB { Property1 = "Str1", NewProperty = "SomeValue" } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DictionaryValueInequality()
        {
            var a1 = new A
            {
                DictionaryOfB = new Dictionary<int, B>
                {
                    { 1, new B {Property1 = "Str1"} },
                    { 2, new B {Property1 = "Str2"} }
                }
            };
            var a2 = new A
            {
                DictionaryOfB = new Dictionary<int, B>
                {
                    { 1, new B {Property1 = "Str1"} },
                    { 2, new B {Property1 = "Str3"} }
                }
            };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("DictionaryOfB[1].Value.Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void DictionaryKeyInequality()
        {
            var a1 = new A
            {
                DictionaryOfB = new Dictionary<int, B>
                {
                    { 1, new B {Property1 = "Str1"} },
                    { 2, new B {Property1 = "Str2"} }
                }
            };
            var a2 = new A
            {
                DictionaryOfB = new Dictionary<int, B>
                {
                    { 1, new B {Property1 = "Str1"} },
                    { 3, new B {Property1 = "Str2"} }
                }
            };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("DictionaryOfB[1].Key", differences.First().MemberPath);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
        }

        [TestCase(FlagsEnum.Flag1 | FlagsEnum.Flag2, FlagsEnum.Flag1 | FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag2, FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag1, FlagsEnum.Flag1 | FlagsEnum.Flag2)]
        public void FlagsInequality(FlagsEnum flags1, FlagsEnum flags2)
        {
            var a1 = new A { Flags = flags1 };
            var a2 = new A { Flags = flags2 };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("Flags", differences.First().MemberPath);
            Assert.AreEqual(flags1.ToString(), differences.First().Value1);
            Assert.AreEqual(flags2.ToString(), differences.First().Value2);
        }

        [TestCase(FlagsEnum.Flag1 | FlagsEnum.Flag2, FlagsEnum.Flag1 | FlagsEnum.Flag2)]
        [TestCase(FlagsEnum.Flag2, FlagsEnum.Flag2)]
        public void FlagsEquality(FlagsEnum flags1, FlagsEnum flags2)
        {
            var a1 = new A { Flags = flags1 };
            var a2 = new A { Flags = flags2 };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void CollectionOfBCountInequality1()
        {
            var a1 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1" } }
            };
            var a2 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1" }, new B { Property1 = "B2" } }
            };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("EnumerableOfB.Count", differences.First().MemberPath);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("2", differences.First().Value2);
        }

        [Test]
        public void CollectionOfBCountInequality2()
        {
            var a1 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1" } }
            };
            var a2 = new A
            {
                EnumerableOfB = new B[0]
            };
            var comparer = new Comparer<A>();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("EnumerableOfB.Count", differences.First().MemberPath);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("0", differences.First().Value2);
        }
    }
}
