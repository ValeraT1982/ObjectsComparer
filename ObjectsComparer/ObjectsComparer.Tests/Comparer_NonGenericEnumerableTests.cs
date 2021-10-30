﻿using System;
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

        [Test]
        public void RightComparisonContextGraph()
        {
            var person1 = new Person
            {
                FirstName = "FirstName1",
                LastName = "LastName 1",
                NonGenericLiveAddresses = new ArrayList
                {
                    new Address
                    {
                        Id = 1,
                        City = "City 1",
                        State = "State 1"
                    },

                    new Address{}
                },
                NonGenericStayAddresses = new ArrayList
                {
                    new Address
                    {
                        Id = 2,
                        City = "City 3"
                    },
                },
            };

            var person2 = new Person
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                NonGenericLiveAddresses = new ArrayList
                {
                    null,

                    new Address
                    {
                        Id = 1,
                        City = "City 2",
                        State = "State 2"
                    },

                    null
                }
            };

            var settings = new ComparisonSettings();

            settings.List.Configure(options =>
            {
                options.CompareUnequalLists = true;
                //options.CompareElementsByKey();
            });

            var comparer = new Comparer<Person>(settings);
            var rootContext = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(person1, person2, rootContext).ToList();
            var hasDiffs = rootContext.HasDifferences(false);
            var scontextBeforeShrink = rootContext.ToJson();
            rootContext.Shrink();
            var scontextAfterShrink = rootContext.ToJson();
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
