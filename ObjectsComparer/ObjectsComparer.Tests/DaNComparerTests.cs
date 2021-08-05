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

    class Osoba
    {
        public int Id { get; set; }

        public string Jmeno { get; set; }

        public List<Adresa> TrvaleAdresy { get; set; } = new List<Adresa>();

        public List<Adresa> PrechodneAdresy { get; set; } = new List<Adresa>();

        public Osoba Pritel { get; set; }
    }

    class Adresa
    {
        public int Id { get; set; }

        public string Ulice { get; set; }

        public List<Mesto> Mesta { get; set; } = new List<Mesto>();
    }

    class Mesto
    {
        public int Id { get; set; }

        public string Nazev { get; set; }
    }

    [TestFixture]
    public class DaNComparerTests
    {
        [Test]
        public void TestOsoba()
        {
            Tuple<Osoba, Osoba> osoby = LoadOsoby();
            var comparer = new Comparer<Osoba>();
            var context = ComparisonContext.Undefined;
            var diffs = comparer.CalculateDifferences(osoby.Item1, osoby.Item2, context).ToArray();
        }

        Tuple<Osoba, Osoba> LoadOsoby()
        {
            var osoba1 = new Osoba
            {
                Id = 1,
                Jmeno = "Daniel",
            };
            var pritel1 = new Osoba
            {
                Id = -1,
                Jmeno = "Pavel"
            };

            osoba1.Pritel = pritel1;
            pritel1.Pritel = osoba1;

            var adresaPritele1 = new Adresa
            {
                Id = 11,
                Ulice = "Bílá"
            };
            osoba1.Pritel.TrvaleAdresy.Add(adresaPritele1);

            var adresa1 = new Adresa
            {
                Id = 2,
                Ulice = "Májová",
            };
            var adresa2 = new Adresa
            {
                Id = 3,
                Ulice = "Bělská"
            };
            var adresa3 = new Adresa
            {
                Id = 4,
                Ulice = "Růžová"
            };
            var adresaList = new List<Adresa>(new Adresa[] { adresa1, adresa2, adresa3 });
            osoba1.TrvaleAdresy.AddRange(adresaList);

            var osoba2 = new Osoba
            {
                Id = 1,
                Jmeno = "Jan",
            };

            var pritel2 = new Osoba
            {
                Id = -2,
                Jmeno = "Petr"
            };

            osoba2.Pritel = pritel2;

            var adresaPritele2 = new Adresa
            {
                Id = 111,
                Ulice = "Černá"
            };
            osoba2.Pritel.TrvaleAdresy.Add(adresaPritele2);

            var adresa4 = new Adresa
            {
                Id = 2,
                Ulice = "Májová"
            };
            var adresa5 = new Adresa
            {
                Id = 3,
                Ulice = "Bělská"
            };
            var adresa6 = new Adresa
            {
                Id = 4,
                Ulice = "Modrá"
            };
            var adresaList2 = new List<Adresa>(new Adresa[] { adresa4, adresa5, adresa6 });
            osoba2.TrvaleAdresy.AddRange(adresaList2);

            return new Tuple<Osoba, Osoba>(osoba1, osoba2);
        }

        public void Calc<T>(T obj1, T obj2)
        {
            var x = ((object)obj1 ?? obj2).GetType();
        }

        [Test]
        public void TestEnumerablesComparer()
        {
            string s1 = null;
            string s2 = "x";
            Calc(s2, s1);

            var car1 = new Car();
            //car1.Items1 = new int[] { 1, 2, 3 };
            //car1.Items2 = new List<string> { "ahoj", "nazdar", "čau" };
            car1.Items3 = new string[] { "ahoj", "nazdar", "čau" };
            var car2 = new Car();
            //car2.Items1 = new int[] { -1, 2, 3 };
            //car2.Items2 = new List<string> { "ahoj", "hello", "čau" };
            car2.Items3 = new string[] { "hi", "nazdar", "čau" };
            //var comparer = new Comparer<Car>();
            var comparer = new Comparer();
            //var diffs = comparer.CalculateDifferences(typeof(Car), car1, car2).ToArray();
            var diffs = comparer.CalculateDifferences(car1, car2).ToArray();
            //var member = car1.GetType().GetMember(nameof(car1.Items1)).Single();
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