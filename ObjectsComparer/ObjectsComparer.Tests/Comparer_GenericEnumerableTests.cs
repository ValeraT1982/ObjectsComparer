using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using System.Collections.Generic;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerGenericEnumerableTests
    {
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
        public void PrimitiveTypeArrayInequalityFirstNull()
        {
            var a1 = new A();
            var a2 = new A { IntArray = new int[0] };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("IntArray", differences.First().MemberPath);
            Assert.AreEqual(string.Empty, differences.First().Value1);
            Assert.AreEqual(a2.IntArray.ToString(), differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences.First().DifferenceType);
        }

        [Test]
        public void PrimitiveTypeArrayInequalitySecondNull()
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
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("CollectionOfB", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);
        }

        [Test]
        public void CollectionAndNullInequality()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("CollectionOfB", differences[0].MemberPath);
            Assert.AreEqual(a1.CollectionOfB.ToString(), differences[0].Value1);
            Assert.AreEqual(string.Empty, differences[0].Value2);
        }

        [Test]
        public void NullAndCollectionInequality()
        {
            var a1 = new A();
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("CollectionOfB", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual(a2.CollectionOfB.ToString(), differences[0].Value2);
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
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("ClassImplementsCollectionOfB", differences[0].MemberPath);
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
        public void NullAndEmptyComparisonGenericInequality()
        {
            var a1 = new A { ListOfB = new List<B>() };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
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

            var isEqual = comparer.Compare(a1, a2, out var differences);

            Assert.IsTrue(isEqual);
            CollectionAssert.IsEmpty(differences);
        }

        [TestCase(FlagsEnum.Flag1 | FlagsEnum.Flag2, FlagsEnum.Flag1 | FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag2, FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag1, FlagsEnum.Flag1 | FlagsEnum.Flag2)]
        public void FlagsInequality(FlagsEnum flags1, FlagsEnum flags2)
        {
            var a1 = new A { Flags = flags1 };
            var a2 = new A { Flags = flags2 };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
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

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("EnumerableOfB", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
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

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("EnumerableOfB", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("0", differences.First().Value2);
        }


        [Test]
        public void HashSetEqualitySameOrder()
        {
            var a1 = new HashSet<string> { "a", "b" };
            var a2 = new HashSet<string> { "a", "b" };
            var comparer = new Comparer<HashSet<string>>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HashSetEqualityDifferentOrder()
        {
            var a1 = new HashSet<string> { "a", "b" };
            var a2 = new HashSet<string> { "b", "a" };
            var comparer = new Comparer<HashSet<string>>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HashSetInequalityDifferentElements()
        {
            var a1 = new HashSet<string> { "a", "b" };
            var a2 = new HashSet<string> { "a", "c" };
            var comparer = new Comparer<HashSet<string>>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == string.Empty && d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.Value2 == "c"));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == string.Empty && d.DifferenceType == DifferenceTypes.MissedElementInSecondObject && d.Value1 == "b"));
        }

        [Test]
        public void HashSetInequalityDifferentNumberOfElements()
        {
            var a1 = new HashSet<string> { "a", "b" };
            var a2 = new HashSet<string> { "a", "b", "c" };
            var comparer = new Comparer<HashSet<string>>();
            
            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == string.Empty && d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.Value2 == "c"));
        }

        [Test]
        public void HashSetAndNullInequality()
        {
            var a = new HashSet<string> { "a", "b" };
            var comparer = new Comparer<HashSet<string>>();

            var isEqual = comparer.Compare(a, null, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == string.Empty && d.DifferenceType == DifferenceTypes.ValueMismatch));
        }

        [Test]
        public void IgnoreCapacityForLists()
        {
            var a1 = new A
            {
                ListOfB = new List<B> { new B { Property1 = "str2" }, new B { Property1 = "str2" } }
            };

            var a2 = new A
            {
                ListOfB = new List<B> { new B { Property1 = "str2" }, new B { Property1 = "str2" } }
            };

            a1.ListOfB.TrimExcess();

            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void CompareAsIList()
        {
            var list1 = new List<int> { 1, 2 };
            var list2 = new List<int> { 1 };

            var comparer = new Comparer<IList<int>>();

            var differences = comparer.CalculateDifferences(list1, list2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
        }
    }
}
