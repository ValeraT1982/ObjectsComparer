﻿using System.Collections;
using System.Linq;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
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

        static string GetMyString() => "xxx";

        [Test]
        public void InequalityCount_InequalityProperty_CompareByKey()
        {
            var a1 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str2" },
                    new B { Property1 = "Str1" } ,
                    null
                }
            };

            var a2 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str1" }
                }
            };

            //ComparisonContext.BelongsTo(): Returns true if current context's Member belongs to the member or if it is the member itself.
            var r = PropertyHelper.GetMemberInfo<string>(() => new B().Property1);

            var settings = new ComparisonSettings();

            settings.List.Configure((ctx, options) =>
            {
                options.CompareUnequalLists = true;
                options.CompareElementsByKey(keyOptions => keyOptions.UseKey(nameof(B.Property1)));
            });

            var comparer = new Comparer<A>(settings);
            var rootCtx = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(a1, a2, rootCtx).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("3", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str2]", differences[1].MemberPath);
            Assert.AreEqual(false, string.IsNullOrWhiteSpace(differences[1].Value1));
            Assert.AreEqual(true, string.IsNullOrWhiteSpace(differences[1].Value2));
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[2].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[NULL]", differences[2].MemberPath);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
        }

        [Test]
        public void InequalityCount_InequalityProperty_CompareByIndex()
        {
            var a1 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str2" },
                    new B { Property1 = "Str1" } ,
                    null
                }
            };

            var a2 = new A 
            {
                NonGenericEnumerable = new ArrayList 
                {
                    new B { Property1 = "Str1" } 
                } 
            };

            var settings = new ComparisonSettings();

            settings.List.Configure((ctx, options) =>
            {
                options.CompareUnequalLists = true;
            });

            var comparer = new Comparer<A>(settings);
            var rootCtx = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(a1, a2, rootCtx).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("3", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[0].Property1", differences[1].MemberPath);
            Assert.AreEqual("Str2", differences[1].Value1);
            Assert.AreEqual("Str1", differences[1].Value2);
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[2].DifferenceType);
            Assert.AreEqual(true, differences[2].Value1 != string.Empty);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
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
