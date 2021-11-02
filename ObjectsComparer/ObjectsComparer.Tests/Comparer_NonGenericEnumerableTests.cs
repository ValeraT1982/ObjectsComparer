using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Tests.Utils;
using ObjectsComparer.Utils;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerNonGenericEnumerableTests
    {
        [Test]
        public void Equality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equality_CompareByKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str2" }, new B { Property1 = "Str1" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => listOptions.CompareElementsByKey(keyOptions => keyOptions.UseKey("Property1")));
            var comparer = new Comparer<A>(settings);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void InequalityCount()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
        }

        [Test]
        public void InequalityCount_CompareUnequalLists()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => listOptions.CompareUnequalLists = true);
            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            var diff2 = differences[1];
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, diff2.DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[1]", diff2.MemberPath);
            Assert.AreNotEqual(string.Empty, diff2.Value1);
            Assert.AreEqual(string.Empty, diff2.Value2);
        }

        [Test]
        public void InequalityCount_CompareUnequalLists_CompareByKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => 
            {
                listOptions.CompareUnequalLists = true;
                listOptions.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.UseKey("Property1");
                    keyOptions.FormatElementKey((elementIndex, elementKey) => $"Property1={elementKey}");
                });
            });
            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            var diff2 = differences[1];
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, diff2.DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Property1=Str2]", diff2.MemberPath);
            Assert.AreNotEqual(string.Empty, diff2.Value1);
            Assert.AreEqual(string.Empty, diff2.Value2);
        }

        [Test]
        public void InequalityCount_CompareUnequalLists_CompareByKey_DontFormatKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions =>
            {
                listOptions.CompareUnequalLists = true;
                listOptions.CompareElementsByKey(keyOptions => keyOptions.UseKey("Property1"));
            });

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            var diff2 = differences[1];
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, diff2.DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str2]", diff2.MemberPath);
            Assert.AreNotEqual(string.Empty, diff2.Value1);
            Assert.AreEqual(string.Empty, diff2.Value2);
        }

        [Test]
        public void InequalityProperty()
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
        public void InequalityProperty_CompareByKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str2", Id = 2 } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str3", Id = 2 }, new B { Property1 = "Str1", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var rootCtx = ComparisonContext.CreateRoot();
            var differences = comparer.CalculateDifferences(a1, a2, rootCtx).ToList();

            CollectionAssert.IsNotEmpty(differences);
            var diff = differences.First();
            Assert.AreEqual("NonGenericEnumerable[2].Property1", diff.MemberPath);
            Assert.AreEqual("Str2", diff.Value1);
            Assert.AreEqual("Str3", diff.Value2);

            var diffFromCtx = rootCtx.GetDifferences(recursive: true).FirstOrDefault();
            Assert.NotNull(diffFromCtx);
            Assert.AreEqual(diff, diffFromCtx);
        }

        [Test]
        public void NullElementsEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { null } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullElementsEquality_CompareUnequalLists()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { null, null } };

            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => listOptions.CompareUnequalLists = true);

            var comparer = new Comparer<A>(settings);
            var isEqual = comparer.Compare(a1, a2, out var diffs);
            var differences = diffs.ToArray();
            Assert.IsTrue(differences.Count() == 2);
            Assert.IsTrue(differences[0].DifferenceType == DifferenceTypes.NumberOfElementsMismatch);
            Assert.IsTrue(differences[0].Value1 == "1");
            Assert.IsTrue(differences[0].Value2 == "2");
            Assert.IsTrue(differences[1].DifferenceType == DifferenceTypes.MissedElementInFirstObject);
            Assert.IsTrue(differences[1].MemberPath == "NonGenericEnumerable[1]");
            Assert.IsTrue(differences[1].Value1 == string.Empty);
            Assert.IsTrue(differences[1].Value2 == string.Empty);
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void NullElementsEquality_CompareUnequalLists_CompareByKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { null, null } };

            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => 
            {
                listOptions.CompareUnequalLists = true;                
                listOptions.CompareElementsByKey(); //Because all elements are null, no key definition is required for this test method.
            });

            var comparer = new Comparer<A>(settings);
            var isEqual = comparer.Compare(a1, a2, out var diffs);
            var differences = diffs.ToArray();
            Assert.IsTrue(differences.Count() == 1);
            Assert.IsTrue(differences[0].DifferenceType == DifferenceTypes.NumberOfElementsMismatch);
            Assert.IsTrue(differences[0].Value1 == "1");
            Assert.IsTrue(differences[0].Value2 == "2");
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void NullAndNotNullElementsInequality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null, "Str1" } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { "Str2", null } };

            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);
            Assert.AreEqual("NonGenericEnumerable[0]", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual("Str2", differences[0].Value2);
            Assert.AreEqual("NonGenericEnumerable[1]", differences[1].MemberPath);
            Assert.AreEqual("Str1", differences[1].Value1);
            Assert.AreEqual(string.Empty, differences[1].Value2);
        }

        [Test]
        public void NullAndNotNullElementsInequality_CompareByKey()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null, "Str1" } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { "Str2", null } };

            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions =>
            {
                listOptions.CompareElementsByKey(); 
            });

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[0].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str1]", differences[0].MemberPath);            
            Assert.AreEqual("Str1", differences[0].Value1);
            Assert.AreEqual(string.Empty, differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str2]", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual("Str2", differences[0].Value2);
        }

        [Test]
        public void InequalityType()
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
        public void DerivedClassEquality()
        {
            var a1 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var a2 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DerivedClassInequalityProperty()
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
        public void NullAndEmptyInequality()
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
        public void NullAndEmptyEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList() };
            var a2 = new A();
            var comparer = new Comparer<A>(new ComparisonSettings { EmptyAndNullEnumerablesEqual = true });

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }
    }
}
