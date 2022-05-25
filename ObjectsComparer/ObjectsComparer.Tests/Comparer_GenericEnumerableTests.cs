using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using System.Collections.Generic;
using ObjectsComparer.Exceptions;
using System;
using ObjectsComparer.Utils;
using ObjectsComparer.Tests.Utils;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.Collections;
using System.Reflection;

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
        public void ValueTypeArrayEquality_CompareByKey()
        {
            var a1 = new A { IntArray = new[] { 2, 1 } };
            var a2 = new A { IntArray = new[] { 1, 2 } };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void PrimitiveTypeArrayInequalityCount_CompareUnequalLists()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 2, 3 } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareUnequalLists: true);

            var comparer = new Comparer<A>(settings);

            var rootNode = comparer.CalculateDifferenceTree(a1, a2);
            var differences = rootNode.GetDifferences(true).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("IntArray[2]", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual("3", differences[0].Value2);            

            Assert.AreEqual("IntArray.Length", differences[1].MemberPath);
            Assert.AreEqual("2", differences[1].Value1);
            Assert.AreEqual("3", differences[1].Value2);
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
        public void PrimitiveTypeArrayInequalityMember_CompareByKey()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.IsTrue(differences.Count == 2);

            var diff1 = differences[0];
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, diff1.DifferenceType);
            Assert.AreEqual("IntArray[2]", diff1.MemberPath);
            Assert.AreEqual("2", diff1.Value1);
            Assert.AreEqual(string.Empty, diff1.Value2);

            var diff2 = differences[1];
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, diff2.DifferenceType);
            Assert.AreEqual("IntArray[3]", diff2.MemberPath);
            Assert.AreEqual(string.Empty, diff2.Value1);
            Assert.AreEqual("3", diff2.Value2);
        }

        [Test]
        public void PrimitiveTypeArrayInequalityMember_CompareByKey_FormatKey()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions =>
            {
                listOptions.CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(args => $"Key={args.ElementKey}"));
            });

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.IsTrue(differences.Count == 2);

            var diff1 = differences[0];
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, diff1.DifferenceType);
            Assert.AreEqual("IntArray[Key=2]", diff1.MemberPath);
            Assert.AreEqual("2", diff1.Value1);
            Assert.AreEqual(string.Empty, diff1.Value2);

            var diff2 = differences[1];
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, diff2.DifferenceType);
            Assert.AreEqual("IntArray[Key=3]", diff2.MemberPath);
            Assert.AreEqual(string.Empty, diff2.Value1);
            Assert.AreEqual("3", diff2.Value2);
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
        public void PrimitiveTypeArrayInequalityFirstNull_CompareBykey()
        {
            var a1 = new A();
            var a2 = new A { IntArray = new int[0] };
            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void PrimitiveTypeArrayInequalitySecondNull_CompareByKey()
        {
            var a1 = new A { IntArray = new int[0] };
            var a2 = new A();

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void ClassArrayEquality_ComareByKey_Throw_ElementKeyNotFoundException()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            Assert.Throws<ElementKeyNotFoundException>(() => 
            {
                var isEqual = comparer.Compare(a1, a2);
            });
        }

        [Test]
        public void ClassArrayEquality_ComareByKey_DoesNotThrow_ElementKeyNotFoundException()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey(keyOptions => keyOptions.ThrowKeyNotFound(false)));

            var comparer = new Comparer<A>(settings);
            bool isEqual = false;

            Assert.DoesNotThrow(() =>
            {
                isEqual = comparer.Compare(a1, a2);
            });

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
        public void ClassArrayInequalityCount_CompareByKey_DoesNotThrow_ElementKeyNotFoundException()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions.ThrowKeyNotFound(false)));

            settings.ConfigureListComparison((currentNode, options) => 
            {
                if (currentNode.Member.Name == "TrvaleAdresy") 
                {
                    options.CompareElementsByKey();
                }
            });

            var comparer = new Comparer<A>(settings);

            List<Difference> differences = null;

            Assert.DoesNotThrow(() => differences = comparer.CalculateDifferences(a1, a2).ToList());

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("ArrayOfB.Length", differences[0].MemberPath);
            Assert.AreEqual("1", differences[0].Value1);
            Assert.AreEqual("2", differences[0].Value2);
        }

        [Test]
        public void ClassArrayInequalityCount_CompareUnequalLists()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("ArrayOfB[1]", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[0].Value2);

            Assert.AreEqual("ArrayOfB.Length", differences[1].MemberPath);
            Assert.AreEqual("1", differences[1].Value1);
            Assert.AreEqual("2", differences[1].Value2);
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
        public void ClassArrayInequalityProperty_CompareByKey()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str2", Id = 1 }, new B { Property1 = "Str1", Id = 2 } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1", Id = 2 }, new B { Property1 = "Str3", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ArrayOfB[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void ClassArrayInequalityProperty_CompareByKey_FormatKey()
        {
            var a1 = new A { ArrayOfB = new[] { new B { Property1 = "Str2", Id = 2 }, new B { Property1 = "Str1", Id = 1 } } };
            var a2 = new A { ArrayOfB = new[] { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str3", Id = 2 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(args => $"Id={args.ElementKey}")));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("ArrayOfB[Id=2].Property1", differences.First().MemberPath);
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
        public void CollectionEquality_CompareByKey()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str2", Id = 1 }, new B { Property1 = "Str1", Id = 2 } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1", Id = 2 }, new B { Property1 = "Str2", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void CollectionInequalityCount_CompareUnequalLists()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("CollectionOfB", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("CollectionOfB[1]", differences[1].MemberPath);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);
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
        public void CollectionAndNullInequality_CompareByKey()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A();

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions =>
            {
                listOptions.CompareElementsByKey();
            });

            var comparer = new Comparer<A>(settings);

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
        public void NullAndCollectionInequality_CompareByKey()
        {
            var a1 = new A();
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions =>
            {
                listOptions.CompareElementsByKey();
            });

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("CollectionOfB", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual(a2.CollectionOfB.ToString(), differences[0].Value2);
        }

        [Test]
        public void IgnoreAttributeComparisonEquality()
        {
            var a1 = new Parent();
            a1.Child1.Add(new ParentChild(a1, "Child1"));
            a1.Child1.Add(new ParentChild(a1, "Child2"));

            var a2 = new Parent();
            a2.Child1.Add(new ParentChild(a2, "Child1"));
            a2.Child1.Add(new ParentChild(a2, "Child2"));

            var comparer = new Comparer<Parent>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void IgnoreAttributeComparisonInEquality()
        {
            var a1 = new Parent();
            a1.Child1.Add(new ParentChild(a1, "Child1"));
            a1.Child1.Add(new ParentChild(a1, "Child2"));

            var a2 = new Parent();
            a2.Child1.Add(new ParentChild(a2, "Child1"));
            a2.Child1.Add(new ParentChild(a2, "Child3"));

            var comparer = new Comparer<Parent>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("Child1[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Child2", differences.First().Value1);
            Assert.AreEqual("Child3", differences.First().Value2);
        }



        [Test]
        public void IgnoreAttributeComparisonDeepEquality()
        {
          var a1 = new Parent();
          a1.Child1.Add(new ParentChild(a1,
                                        "Child1",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));
          a1.Child1.Add(new ParentChild(a1,
                                        "Child2",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));

          var a2 = new Parent();
          a2.Child1.Add(new ParentChild(a1,
                                        "Child1",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));
          a2.Child1.Add(new ParentChild(a1,
                                        "Child2",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p5")
                                        }
                                       ));

          var comparer = new Comparer <Parent>();

          var isEqual = comparer.Compare(a1,
                                         a2);

          Assert.IsTrue(isEqual);
        }



        [Test]
        public void IgnoreAttributeComparisonDeepInEquality()
        {
          var a1 = new Parent();
          a1.Child1.Add(new ParentChild(a1,
                                        "Child1",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));
          a1.Child1.Add(new ParentChild(a1,
                                        "Child2",
                                        new ObservableCollection <Child>()
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));

          var a2 = new Parent();
          a2.Child1.Add(new ParentChild(a1,
                                        "Child1",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p3",
                                                      "p4")
                                        }
                                       ));
          a2.Child1.Add(new ParentChild(a1,
                                        "Child2",
                                        new ObservableCollection <Child>
                                        {
                                            new Child("p1",
                                                      "p2"),
                                            new Child("p5",
                                                      "p6")
                                        }
                                       ));

          var comparer = new Comparer <Parent>();

          var differences = comparer.CalculateDifferences(a1,
                                                          a2).ToList();

          CollectionAssert.IsNotEmpty(differences);
          Assert.AreEqual("Child1[1].Children[1].Property1",
                          differences.First().MemberPath);
          Assert.AreEqual("p3",
                          differences.First().Value1);
          Assert.AreEqual("p5",
                          differences.First().Value2);
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
        public void CollectionInequalityProperty_CompareByKey()
        {
            var a1 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str2", Id = 1 }, new B { Property1 = "Str1", Id = 2 } } };
            var a2 = new A { CollectionOfB = new Collection<B> { new B { Property1 = "Str1", Id = 2 }, new B { Property1 = "Str3", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());
            
            var comparer = new Comparer<A>(settings);

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
        public void ClassImplementsCollectionEquality_CompareByKey()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str2", Id = 2 } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str2", Id = 2 }, new B { Property1 = "Str1", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void ClassImplementsCollectionInequalityCount_CompareUnequalLists()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("ClassImplementsCollectionOfB", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("ClassImplementsCollectionOfB[1]", differences[1].MemberPath);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);
        }

        [Test]
        public void ClassImplementsCollectionInequalityCount_CompareUnequalLists_CompareByKey()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str2", Id = 2 } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1", Id = 1 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("ClassImplementsCollectionOfB", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("ClassImplementsCollectionOfB[2]", differences[1].MemberPath);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);
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
        public void ClassImplementsCollectionInequalityProperty_CompareByKey()
        {
            var a1 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str2", Id = 2 } } };
            var a2 = new A { ClassImplementsCollectionOfB = new CollectionOfB { new B { Property1 = "Str1", Id = 1 }, new B { Property1 = "Str3", Id = 2 } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassImplementsCollectionOfB[2].Property1", differences.First().MemberPath);
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
        public void CollectionOfBCountInequality1_CompareElementsByKey()
        {
            var a1 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1" } }
            };
            var a2 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1" }, new B { Property1 = "B2" } }
            };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

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
        public void CollectionOfBCountInequality1_CompareElementsByKey_CompareUnequalLists()
        {
            var a1 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1", Id = 1 } }
            };
            var a2 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1", Id = 1 }, new B { Property1 = "B2", Id = 2 } }
            };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer<A>(settings);

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual("EnumerableOfB", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("2", differences.First().Value2);

            Assert.AreEqual("EnumerableOfB[2]", differences[1].MemberPath);
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[1].Value2);
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
        public void CollectionOfBCountInequality2_CompareByKey()
        {
            var a1 = new A
            {
                EnumerableOfB = new[] { new B { Property1 = "B1", Id = 1 } }
            };
            var a2 = new A
            {
                EnumerableOfB = new B[0]
            };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());
            var comparer = new Comparer<A>(settings);

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual("EnumerableOfB", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("0", differences.First().Value2);

            Assert.AreEqual("EnumerableOfB[1]", differences[1].MemberPath);
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);
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

        [Test]
        public void PrimitiveTypeArray_CompareByKey_CompareUnequalLists_Ignore_Repeated_Elements()
        {
            var a1 = new A() { IntArray = new int[] { 1, 2 } };
            var a2 = new A() { IntArray = new int[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey().CompareUnequalLists(true));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.IsTrue(differences.Count == 1);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[0].DifferenceType);
            Assert.AreEqual("IntArray.Length", differences[0].MemberPath);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("10", differences[0].Value2);
        }

        [Test]
        public void PrimitiveTypeArray_CompareByIndex_CompareUnequalLists_Dont_Ignore_Repeated_Elements()
        {
            var a1 = new A() { IntArray = new int[] { 1, 2 } };
            var a2 = new A() { IntArray = new int[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer<A>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.IsTrue(differences.Count == 9);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("IntArray[2]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("IntArray[3]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("2", differences[1].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[2].DifferenceType);
            Assert.AreEqual("IntArray[4]", differences[2].MemberPath);
            Assert.AreEqual("", differences[2].Value1);
            Assert.AreEqual("1", differences[2].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[3].DifferenceType);
            Assert.AreEqual("IntArray[5]", differences[3].MemberPath);
            Assert.AreEqual("", differences[3].Value1);
            Assert.AreEqual("2", differences[3].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[4].DifferenceType);
            Assert.AreEqual("IntArray[6]", differences[4].MemberPath);
            Assert.AreEqual("", differences[4].Value1);
            Assert.AreEqual("1", differences[4].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[5].DifferenceType);
            Assert.AreEqual("IntArray[7]", differences[5].MemberPath);
            Assert.AreEqual("", differences[5].Value1);
            Assert.AreEqual("2", differences[5].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[6].DifferenceType);
            Assert.AreEqual("IntArray[8]", differences[6].MemberPath);
            Assert.AreEqual("", differences[6].Value1);
            Assert.AreEqual("1", differences[6].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[7].DifferenceType);
            Assert.AreEqual("IntArray[9]", differences[7].MemberPath);
            Assert.AreEqual("", differences[7].Value1);
            Assert.AreEqual("2", differences[7].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[8].DifferenceType);
            Assert.AreEqual("IntArray.Length", differences[8].MemberPath);
            Assert.AreEqual("2", differences[8].Value1);
            Assert.AreEqual("10", differences[8].Value2);
        }

        [Test]
        public void CompareAsIList_CompareUnequalLists()
        {
            var list1 = new List<int> { 1, 2 };
            var list2 = new List<int> { 1 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer<IList<int>>(settings);

            var differences = comparer.CalculateDifferences(list1, list2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("[1]", differences[1].MemberPath);
            Assert.AreEqual("2", differences[1].Value1);
            Assert.AreEqual(string.Empty, differences[1].Value2);
        }

        [Test]
        public void CompareAsIList_CompareUnequalLists_CompareElementsByKey()
        {
            var list1 = new List<int> { 1, 2 };
            var list2 = new List<int> { 1 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer<IList<int>>(settings);

            var differences = comparer.CalculateDifferences(list1, list2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("[2]", differences[1].MemberPath);
            Assert.AreEqual("2", differences[1].Value1);
            Assert.AreEqual(string.Empty, differences[1].Value2);
        }

        [Test]
        public void CompareAsIList_CompareUnequalLists_CompareElementsByKey_FormatKey()
        {
            var list1 = new List<int> { 1, 2 };
            var list2 = new List<int> { 1 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions =>
            {
                //DaN Fluent.
                //listOptions.CompareUnequalLists().CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(elementKey => $"Key={elementKey}"));

                /*
                 * listOptions
                 *  .CompareUnequalLists()
                 *  .CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(elementKey => $"Key={elementKey}"));
                 */

                listOptions.CompareUnequalLists(true);

                listOptions.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.FormatElementKey(args =>
                    {
                        return $"Key={args.ElementKey}";
                    });
                });
            });

            var comparer = new Comparer<IList<int>>(settings);

            var differences = comparer.CalculateDifferences(list1, list2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("[Key=2]", differences[1].MemberPath);
            Assert.AreEqual("2", differences[1].Value1);
            Assert.AreEqual(string.Empty, differences[1].Value2);
        }

        [Test]
        public void DictionaryEqualitySameOrder()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var comparer = new Comparer<Dictionary<int, string>>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DictionaryEqualitySameOrder_CompareByKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var isEqual = comparer.Compare(a1, a2, out var differences);
            var diffs = differences.ToArray();

            Assert.IsTrue(isEqual);
        }
        
        [Test]
        public void DictionaryInequalityDifferentOrder()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 2, "Two" }, { 1, "One" } };
            var comparer = new Comparer<Dictionary<int, string>>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsFalse(isEqual);
        }

        [Test]
        public void DictionaryEqualityDifferentOrder_CompareByKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 2, "Two" }, { 1, "One" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DictionaryInequalityDifferentNumberOfElements()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } };
            var comparer = new Comparer<Dictionary<int, string>>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
        }

        [Test]
        public void DictionaryInequalityDifferentNumberOfElements_CompareByKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
        }

        [Test]
        public void DictionaryInequalityDifferentNumberOfElements_CompareByKey_CompareUnequalLists()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareElementsByKey: true, compareUnequalLists: true);

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("3", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[3]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("[3, Three]", differences[1].Value2);
        }

        [Test]
        public void DictionaryInequalityDifferentNumberOfElements_CompareByKey_CompareUnequalLists_FormatKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(keyArgs => $"Key={keyArgs.ElementKey}")));

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("3", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[Key=3]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("[3, Three]", differences[1].Value2);
        }

        [Test]
        public void DictionaryInequalityDifferentNumberOfElements_CompareByIndex_CompareUnequalLists()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareElementsByKey: false, compareUnequalLists: true);

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("2", differences[0].Value1);
            Assert.AreEqual("3", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[2]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("[3, Three]", differences[1].Value2);
        }

        [Test]
        public void DictionaryInequalityDifferentValue()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two!" } };
            var comparer = new Comparer<Dictionary<int, string>>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences.First().DifferenceType);
            Assert.AreEqual("Two", differences.First().Value1);
            Assert.AreEqual("Two!", differences.First().Value2);
            Assert.AreEqual("[1].Value", differences.First().MemberPath);
        }

        [Test]
        public void DictionaryInequalityDifferentValue_CompareByKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two!" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions.CompareElementsByKey());

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences.First().DifferenceType);
            Assert.AreEqual("Two", differences.First().Value1);
            Assert.AreEqual("Two!", differences.First().Value2);
            Assert.AreEqual("[2].Value", differences.First().MemberPath);
        }

        [Test]
        public void DictionaryInequalityDifferentValue_CompareByKey_FormatElementKey()
        {
            var a1 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
            var a2 = new Dictionary<int, string> { { 1, "One" }, { 2, "Two!" } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions
                .CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(args => $"Key={args.ElementKey}")));

            var comparer = new Comparer<Dictionary<int, string>>(settings);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences.First().DifferenceType);
            Assert.AreEqual("Two", differences.First().Value1);
            Assert.AreEqual("Two!", differences.First().Value2);
            Assert.AreEqual("[Key=2].Value", differences.First().MemberPath);
        }

        [Test]
        public void CompareIntArrayDefaultBehavior()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var comparer = new Comparer();
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 1);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[0].DifferenceType);
            Assert.AreEqual("Length", differences[0].MemberPath);
            Assert.AreEqual("3", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);
        }

        [Test]
        public void CompareIntArrayUnequalListEnabled()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareUnequalLists: true);
            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 4);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "[0]" && d.Value1 == "3" && d.Value2 == "1"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "[2]" && d.Value1 == "1" && d.Value2 == "3"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.MemberPath == "[3]" && d.Value1 == "" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Length" && d.Value1 == "3" && d.Value2 == "4"));
        }

        [Test]
        public void CompareIntArrayByKey()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareElementsByKey: true);
            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 1);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[0].DifferenceType);
            Assert.AreEqual("Length", differences[0].MemberPath);
            Assert.AreEqual("3", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);
        }

        [Test]
        public void CompareIntArrayByKey_UnequalListEnabled()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareElementsByKey: true, compareUnequalLists: true);
            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 2);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.MemberPath == "[4]" && d.Value1 == "" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Length" && d.Value1 == "3" && d.Value2 == "4"));
        }

        [Test]
        public void CompareIntArrayByKeyDisplayIndex()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison(listOptions =>
            {
                listOptions
                    .CompareUnequalLists(true)
                    .CompareElementsByKey(keyOptions => 
                    {
                        keyOptions.FormatElementKey(args => args.ElementIndex.ToString());
                    });
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 2);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.MemberPath == "[3]" && d.Value1 == "" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "Length" && d.Value1 == "3" && d.Value2 == "4"));
        }

        [Test]
        public void CompareObjectListByKey()
        {
            var a1 = new A { ListOfB = new List<B> { new B { Id = 1, Property1 = "Value 1" }, new B { Id = 2, Property1 = "Value 2" } } };
            var a2 = new A { ListOfB = new List<B> { new B { Id = 2, Property1 = "Value two" }, new B { Id = 1, Property1 = "Value one" } } };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareElementsByKey: true);
            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 2);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[1].Property1" && d.Value1 == "Value 1" && d.Value2 == "Value one"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[2].Property1" && d.Value1 == "Value 2" && d.Value2 == "Value two"));
        }

        [Test]
        public void CompareObjectListByCustomKey()
        {
            var a1 = new A { ListOfB = new List<B> { new B { Id = 1, Property1 = "Value 1", Property2 = "Key1" }, new B { Id = 2, Property1 = "Value 2", Property2 = "Key2" } } };
            var a2 = new A { ListOfB = new List<B> { new B { Id = 1, Property1 = "Value two", Property2 = "Key2" }, new B { Id = 2, Property1 = "Value one", Property2 = "Key1" } } };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison(options => 
            {
                options.CompareElementsByKey(keyOptions => 
                {
                    keyOptions.UseKey(nameof(B.Property2));
                });
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 4);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[Key1].Property1" && d.Value1 == "Value 1" && d.Value2 == "Value one"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[Key1].Id.Value" && d.Value1 == "1" && d.Value2 == "2"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[Key2].Property1" && d.Value1 == "Value 2" && d.Value2 == "Value two"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[Key2].Id.Value" && d.Value1 == "2" && d.Value2 == "1"));
        }

        [Test]
        public void CompareIntArrayFirstByIndexSecondByKey()
        {
            var a1 = new A { IntArray = new int[] { 3, 2, 1 }, IntArray2 = new int[] { 3, 2, 1 } };
            var a2 = new A { IntArray = new int[] { 1, 2, 3, 4 }, IntArray2 = new int[] { 1, 2, 3, 4 } };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison((currentProperty, listOptions) =>
            {
                listOptions.CompareUnequalLists(true);

                if (currentProperty.Member.Name == nameof(A.IntArray2))
                {
                    listOptions.CompareElementsByKey();
                }
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 6);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "IntArray[0]" && d.Value1 == "3" && d.Value2 == "1"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "IntArray[2]" && d.Value1 == "1" && d.Value2 == "3"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.MemberPath == "IntArray[3]" && d.Value1 == "" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "IntArray.Length" && d.Value1 == "3" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.MissedElementInFirstObject && d.MemberPath == "IntArray2[4]" && d.Value1 == "" && d.Value2 == "4"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "IntArray2.Length" && d.Value1 == "3" && d.Value2 == "4"));
        }

        [Test]
        public void CompareObjectListFirstByDefaultKeySecondByCustomKey()
        {
            var a1 = new A 
            {
                ListOfB = new List<B> { new B { Id = 1, Property1 = "Value 1" }, new B { Id = 2, Property1 = "Value 2" } },
                ListOfC = new List<C> { new C { Key = "Key1", Property1 = "Value 3" }, new C { Key = "Key2", Property1 = "Value 4" } }
            };

            var a2 = new A 
            {
                ListOfB = new List<B> { new B { Id = 2, Property1 = "Value two" }, new B { Id = 1, Property1 = "Value one" } } ,
                ListOfC = new List<C> { new C { Key = "Key2", Property1 = "Value four" }, new C { Key = "Key1", Property1 = "Value three" } }
            };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison((currentProperty, listOptions) =>
            {
                listOptions.CompareElementsByKey();

                if (currentProperty.Member.Name == nameof(A.ListOfC))
                {
                    listOptions.CompareElementsByKey(keyOptions => keyOptions.UseKey(args => ((C)args.Element).Key));
                }
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 4);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[1].Property1" && d.Value1 == "Value 1" && d.Value2 == "Value one"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[2].Property1" && d.Value1 == "Value 2" && d.Value2 == "Value two"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfC[{ Key = Key1 }].Property1" && d.Value1 == "Value 3" && d.Value2 == "Value three"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfC[{ Key = Key2 }].Property1" && d.Value1 == "Value 4" && d.Value2 == "Value four"));
        }

        [Test]
        public void CompareObjectListFirstByDefaultKeySecondByCustomKeyFormatCustomKey()
        {
            var a1 = new A
            {
                ListOfB = new List<B> { new B { Id = 1, Property1 = "Value 1" }, new B { Id = 2, Property1 = "Value 2" } },
                ListOfC = new List<C> { new C { Key = "Key1", Property1 = "Value 3" }, new C { Key = "Key2", Property1 = "Value 4" } }
            };

            var a2 = new A
            {
                ListOfB = new List<B> { new B { Id = 2, Property1 = "Value two" }, new B { Id = 1, Property1 = "Value one" } },
                ListOfC = new List<C> { new C { Key = "Key2", Property1 = "Value four" }, new C { Key = "Key1", Property1 = "Value three" } }
            };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison((currentProperty, listOptions) =>
            {
                listOptions.CompareElementsByKey();

                if (currentProperty.Member.Name == nameof(A.ListOfC))
                {
                    listOptions.CompareElementsByKey(keyOptions =>
                        keyOptions
                            .UseKey(args => new { ((C)args.Element).Key })
                            .FormatElementKey(args => ((C)args.Element).Key));
                }
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 4);
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[1].Property1" && d.Value1 == "Value 1" && d.Value2 == "Value one"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfB[2].Property1" && d.Value1 == "Value 2" && d.Value2 == "Value two"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfC[{ Key = Key1 }].Property1" && d.Value1 == "Value 3" && d.Value2 == "Value three"));
            Assert.IsTrue(differences.Any(d => d.DifferenceType == DifferenceTypes.ValueMismatch && d.MemberPath == "ListOfC[{ Key = Key2 }].Property1" && d.Value1 == "Value 4" && d.Value2 == "Value four"));
        }

        [Test]
        public void TestCompareEntireObject()
        {
            var a1 = new TestClass { IntProperty = 10, StringProperty1 = "a", StringProperty2 = "c", ClassBProperty = new TestClassB { IntPropertyB = 30 } };
            var a2 = new TestClass { IntProperty = 10, StringProperty1 = "b", StringProperty2 = "c", ClassBProperty = new TestClassB { IntPropertyB = 40 } };

            var comparer = new Comparer<TestClass>();
            //var comparer = new Comparer();
            bool compareResult = comparer.Compare(a1, a2); //Only IntProperty.
            //bool calculateAnyDifferencesResult = comparer.CalculateDifferences(a1, a2).Any(); //Only IntProperty.
            //var diffsArray = comparer.CalculateDifferences(a1, a2).ToArray(); //All properties.

            /*
                IntProperty 10.
                IntProperty 20.
             */
        }

        public class TestClassB 
        {
            int _intPropertyB;

            public int IntPropertyB
            {
                get
                {
                    Console.WriteLine($"C IntPropertyB {_intPropertyB}.");
                    Debug.WriteLine($"IntPropertyB {_intPropertyB}.");
                    return _intPropertyB;
                }

                set => _intPropertyB = value;
            }
        }

        public class TestClass
        {
            int _intProperty;
            string _stringProperty1;
            string _stringProperty2;
            TestClassB _classBProperty;            

            public int IntProperty
            {
                get
                {
                    Console.WriteLine($"C IntProperty {_intProperty}.");
                    Debug.WriteLine($"IntProperty {_intProperty}.");
                    return _intProperty;
                }

                set => _intProperty = value;
            }

            public string StringProperty1
            {
                get
                {
                    Console.WriteLine($"C StringProperty1 {_stringProperty1}.");
                    Debug.WriteLine($"StringProperty1 {_stringProperty1}.");
                    return _stringProperty1;
                }

                set => _stringProperty1 = value;
            }

            public string StringProperty2
            {
                get
                {
                    Console.WriteLine($"C StringProperty2 {_stringProperty2}.");
                    Debug.WriteLine($"StringProperty2 {_stringProperty2}.");
                    return _stringProperty2;
                }

                set => _stringProperty2 = value;
            }

            public TestClassB ClassBProperty
            {
                get
                {
                    Debug.WriteLine($"ClassBProperty {_classBProperty}.");
                    return _classBProperty;
                }

                set => _classBProperty = value;
            }
        }

        [Test]
        public void CompareObjectListByContext()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prag", Country = "Czech republic" },
                        new Address { City = "Prag", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czech republic 2" },
                        new Address { City = "Prag", Country = "Czech republic" }
                    }
                }
            };

            var customer = new Customer
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czech republic 2" }
                    }
                }
            };

            var comparer = new ComparersFactory().GetObjectsComparer<Student>();
            var rootNode = comparer.CalculateDifferenceTree(student1, student2);
            var diffs = rootNode.GetDifferences(true).ToArray();

            var stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            var differenceTreeStr = stringBuilder.ToString();
            var differenceTreeJson = (rootNode as DifferenceTreeNode).ToJson();

            rootNode.Shrink();
            
            Debug.WriteLine("");

            stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            differenceTreeStr = stringBuilder.ToString();
            differenceTreeJson = (rootNode as DifferenceTreeNode).ToJson();
        }

        void WalkDifferenceTree(IDifferenceTreeNode node, int level, StringBuilder stringBuilder)
        {
            var blankMemberName = "?";
            string indent = String.Concat(Enumerable.Repeat(" ", 2 * level));

            if (TreeNodeIsListItem(node) == false)
            {
                var memberName = node?.Member?.Name ?? blankMemberName;
                var line = indent + memberName;
                stringBuilder.AppendLine(line);
                Debug.WriteLine(line);
            }

            foreach (var diff in node.Differences)
            {
                var line = indent + String.Concat(Enumerable.Repeat(" ", 2)) + diff.ToString();
                stringBuilder.AppendLine(line);
                Debug.WriteLine(line);
            }

            level++;

            var descendants = node.Descendants.ToArray();

            for (int i = 0; i < descendants.Length; i++)
            {
                var desc = descendants[i];

                if (TreeNodeIsListItem(desc))
                {
                    var line = indent + String.Concat(Enumerable.Repeat(" ", 2)) + $"[{GetIndex(desc)}]";
                    stringBuilder.AppendLine(line);
                    Debug.WriteLine(line);
                }

                WalkDifferenceTree(desc, level, stringBuilder);
            }
        }

        int? GetIndex(IDifferenceTreeNode node)
        {
            var itemx = node.Ancestor.Descendants
                    .Select((descendant, index) => new { Index = index, Descendant = descendant }).Where(n => n.Descendant == node)
                    .FirstOrDefault();

            return itemx?.Index;
        }

        bool TreeNodeIsListItem(IDifferenceTreeNode node)
        {
            if (node.Ancestor?.Member?.Info is PropertyInfo pi && typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
            {
                return true;
            }

            return false;
        }

        [Test]
        public void CompareObjectListsByComplexKey()
        {
            var a1 = new A
            {
                ListOfC = new List<C> 
                {
                    new C { Property1 = "Key1a", Property2 ="Key1b", Property3 = "Value 1" }, 
                    new C { Property1 = "Key2a", Property2 = "Key2b", Property3 = "Value 2" } 
                }
            };

            var a2 = new A
            {
                ListOfC = new List<C>
                {                    
                    new C { Property1 = "Key2a", Property2 = "Key2b", Property3 = "Value two" },
                    new C { Property1 = "Key1a", Property2 ="Key1b", Property3 = "Value 1" },
                }
            };

            var settings = new ComparisonSettings();

            settings.ConfigureListComparison(listOptions =>
            {
                listOptions.CompareElementsByKey(keyOptions => keyOptions.UseKey(args => new 
                { 
                    ((C)args.Element).Property1, 
                    ((C)args.Element).Property2 
                }));
            });

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToArray();

            Assert.IsTrue(differences.Count() == 1);
            Assert.IsTrue(differences.Any(d => 
                d.DifferenceType == DifferenceTypes.ValueMismatch 
                && d.MemberPath == "ListOfC[{ Property1 = Key2a, Property2 = Key2b }].Property3" 
                && d.Value1 == "Value 2" && 
                d.Value2 == "Value two"));
        }
    }
}
