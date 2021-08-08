using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    public class Driver
    {
    }

    public class Car
    {
        //public IEnumerable Items1 { get; set; }

        //public IEnumerable<string> Items2 { get; set; }

        public string[] Items3 { get; set; }
    }

    class Person
    {
        public int Id { get; set; }

        public string PersonName { get; set; }

        public List<Address> AddressList1 { get; set; } = new List<Address>();

        public List<Address> AddressList2 { get; set; } = new List<Address>();

        public Person BestFriend { get; set; }

        public string[] ShortNames { get; set; }
    }

    class Address
    {
        public int Id { get; set; }

        public string Street { get; set; }

        public List<City> CityList { get; set; } = new List<City>();
    }

    class City
    {
        public int Id { get; set; }

        public string CityName { get; set; }
    }

    [TestFixture]
    public class DaNComparerTests
    {
        [Test]
        public void TestOsoba()
        {
            Tuple<Person, Person> osoby = LoadOsoby();
            var comparer = new Comparer<Person>();
            var rootContext = ComparisonContext.Create();
            var diffs = comparer.CalculateDifferences(osoby.Item1, osoby.Item2, rootContext).ToArray();
        }

        Tuple<Person, Person> LoadOsoby()
        {
            var person1 = new Person
            {
                Id = 1,
                PersonName = "Daniel",
                ShortNames = new string[] { "shn1", "shn2", "shn3" }
            };
            var pritel1 = new Person
            {
                Id = -1,
                PersonName = "Paul"
            };

            person1.BestFriend = pritel1;

            var friend1Address = new Address
            {
                Id = 11,
                Street = "White"
            };
            person1.BestFriend.AddressList1.Add(friend1Address);

            var adr1 = new Address
            {
                Id = 2,
                Street = "Red",
            };
            var adr2 = new Address
            {
                Id = 3,
                Street = "Yellow"
            };
            var adr3 = new Address
            {
                Id = 4,
                Street = "Rose"
            };
            var addressList = new List<Address>(new Address[] { adr1, adr2, adr3 });
            person1.AddressList1.AddRange(addressList);

            var person2 = new Person
            {
                Id = 1,
                PersonName = "John",
                ShortNames = new string[] { "shn1", "shn2", "shn3" }
            };

            var pritel2 = new Person
            {
                Id = -2,
                PersonName = "Peter"
            };

            person2.BestFriend = pritel2;

            var adresaPritele2 = new Address
            {
                Id = 111,
                Street = "Black"
            };
            person2.BestFriend.AddressList1.Add(adresaPritele2);

            var adr4 = new Address
            {
                Id = 2,
                Street = "Red"
            };
            var adr5 = new Address
            {
                Id = 3,
                Street = "Yellow"
            };
            var adr6 = new Address
            {
                Id = 4,
                Street = "Blue"
            };
            var addressList2 = new List<Address>(new Address[] { adr4, adr5, adr6 });
            person2.AddressList1.AddRange(addressList2);

            return new Tuple<Person, Person>(person1, person2);
        }

        public void Calc<T>(T obj1, T obj2)
        {
            var x = ((object)obj1 ?? obj2).GetType();
        }

        public class SubClassA
        {
            public bool BoolProperty { get; set; }
        }

        [Test]
        public void TestBug()
        {
            var comparer = new Comparer();
        }

        [Test]
        public void PropertyEquality()
        {
            var a1 = new A {IntProperty = 10, DateTimeProperty = new DateTime(2017, 1, 1), Property3 = 5};
            var a2 = new A {IntProperty = 10, DateTimeProperty = new DateTime(2017, 1, 1), Property3 = 8};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void PropertyInequality()
        {
            var date1 = new DateTime(2017, 1, 1);
            var date2 = new DateTime(2017, 1, 2);
            var a1 = new A {IntProperty = 10, DateTimeProperty = date1};
            var a2 = new A {IntProperty = 8, DateTimeProperty = date2};
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
            var a1 = new A {Field = 9};
            var a2 = new A {Field = 9};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void FieldInequality()
        {
            var a1 = new A {Field = 10};
            var a2 = new A {Field = 8};
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
            var a1 = new A {ClassB = new B {Property1 = "Str1"}};
            var a2 = new A {ClassB = new B {Property1 = "Str1"}};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void ClassPropertyInequality()
        {
            var a1 = new A {ClassB = new B {Property1 = "Str1"}};
            var a2 = new A {ClassB = new B {Property1 = "Str2"}};
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
            var a2 = new A {ClassB = new B {Property1 = "Str2"}};
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
            var a1 = new A {ClassB = new B {Property1 = "Str2"}};
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
            var a1 = new A {ClassB = new B {Property1 = "Str1"}};
            var a2 = new A {ClassB = new B {Property1 = "Str2"}};
            var comparer = new Comparer<A>(new ComparisonSettings {RecursiveComparison = false});

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void InterfacePropertyEquality()
        {
            var a1 = new A {IntefaceProperty = new TestInterfaceImplementation1 {Property = "Str1"}};
            var a2 = new A
            {
                IntefaceProperty = new TestInterfaceImplementation2 {Property = "Str1", AnotherProperty = 50}
            };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void InterfacePropertyInequality()
        {
            var a1 = new A {IntefaceProperty = new TestInterfaceImplementation1 {Property = "Str1"}};
            var a2 = new A
            {
                IntefaceProperty = new TestInterfaceImplementation2 {Property = "Str2", AnotherProperty = 50}
            };
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
            var a1 = new A {StructProperty = new TestStruct {FieldA = "FA", FieldB = "FB"}};
            var a2 = new A {StructProperty = new TestStruct {FieldA = "FA", FieldB = "FB"}};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void StructPropertyInequality()
        {
            var a1 = new A {StructProperty = new TestStruct {FieldA = "FA", FieldB = "FB"}};
            var a2 = new A {StructProperty = new TestStruct {FieldA = "FA", FieldB = "FBB"}};
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
            var a1 = new A {EnumProperty = TestEnum.Value1};
            var a2 = new A {EnumProperty = TestEnum.Value1};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void EnumPropertyInequality()
        {
            var a1 = new A {EnumProperty = TestEnum.Value1};
            var a2 = new A {EnumProperty = TestEnum.Value2};
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
            var a1 = new A {TestProperty1 = "ABC", Field = 5};
            var a2 = new A {TestProperty1 = "BCD", Field = 6};
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
        public void InheritedAndBaseClassInequality()
        {
            var a1 = new A {ClassB = new B {Property1 = "Str1"}};
            var a2 = new A {ClassB = new InheritedFromB {Property1 = "Str2", NewProperty = "SomeValue"}};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
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
            var a1 = new A {ClassB = new B {Property1 = "Str1"}};
            var a2 = new A {ClassB = new InheritedFromB {Property1 = "Str1", NewProperty = "SomeValue"}};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [TestCase(FlagsEnum.Flag1 | FlagsEnum.Flag2, FlagsEnum.Flag1 | FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag2, FlagsEnum.Flag3)]
        [TestCase(FlagsEnum.Flag1, FlagsEnum.Flag1 | FlagsEnum.Flag2)]
        public void FlagsInequality(FlagsEnum flags1, FlagsEnum flags2)
        {
            var a1 = new A {Flags = flags1};
            var a2 = new A {Flags = flags2};
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
            var a1 = new A {Flags = flags1};
            var a2 = new A {Flags = flags2};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void TypePropertyEquality()
        {
            var a1 = new A {TypeProperty = typeof(string)};
            var a2 = new A {TypeProperty = typeof(string)};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void TypePropertyInequality()
        {
            var a1 = new A {TypeProperty = typeof(string)};
            var a2 = new A {TypeProperty = typeof(int)};
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual("TypeProperty", differences.First().MemberPath);
        }

        [Test]
        public void TimeSpanEquality()
        {
            var a1 = new TimeSpan(123456789);
            var a2 = new TimeSpan(123456789);
            var comparer = new Comparer<TimeSpan>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void TimeSpanInequality()
        {
            var a1 = new TimeSpan(123456789);
            var a2 = new TimeSpan(123456788);
            var comparer = new Comparer<TimeSpan>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(string.Empty, differences.First().MemberPath);
        }

        [Test]
        public void GuidEquality()
        {
            var a1 = new Guid("01234567890123456789012345678912");
            var a2 = new Guid("01234567890123456789012345678912");
            var comparer = new Comparer<Guid>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void GuidInequality()
        {
            var a1 = new Guid("01234567890123456789012345678912");
            var a2 = new Guid("01234567890123456789012345678913");
            var comparer = new Comparer<Guid>();

            var isEqual = comparer.Compare(a1, a2, out var differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(string.Empty, differences.First().MemberPath);
        }
    }
}