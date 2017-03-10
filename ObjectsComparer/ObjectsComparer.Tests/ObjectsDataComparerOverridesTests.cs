using System.Linq;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ObjectsDataComparerOverridesTests
    {
        [Test]
        public void OverrideStringComparisonEqual()
        {
            var a1 = new A { TestProperty1 = "ABC", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { TestProperty1 = "BCD", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str2" } };
            var comparer = new ObjectsDataComparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.AddComparerOverride<string>(stringComparer);

            var differences = comparer.Compare(a1, a2);

            CollectionAssert.IsEmpty(differences);
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideStringComparisonNotEqual()
        {
            var a1 = new A { TestProperty1 = "ABC", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { TestProperty1 = "BCD", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var comparer = new ObjectsDataComparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride(typeof(string), stringComparer);

            var differences = comparer.Compare(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "TestProperty1" && d.Value1 == "ABC" && d.Value2 == "BCD"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "TestProperty2" && d.Value1 == "ABC" && d.Value2 == "ABC"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "ClassB.Property1" && d.Value1 == "Str1" && d.Value2 == "Str1"));
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
            stringComparer.Received().ToString(Arg.Any<object>());
        }

        [Test]
        public void OverrideIntComparisonNotEqual()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };
            var comparer = new ObjectsDataComparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride<int>(stringComparer);

            var differences = comparer.Compare(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "IntArray[0]" && d.Value1 == "1" && d.Value2 == "1"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "IntArray[1]" && d.Value1 == "2" && d.Value2 == "3"));
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
            stringComparer.Received().ToString(Arg.Any<object>());
        }

        [Test]
        public void OverrideIntComparisonEqual()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };
            var comparer = new ObjectsDataComparer<A>();
            var intComparer = Substitute.For<IValueComparer>();
            intComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            intComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride(typeof(int), intComparer);

            var differences = comparer.Compare(a1, a2);

            CollectionAssert.IsEmpty(differences);
            intComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverridePropertyComparisonEqual()
        {
            var a1 = new A { TestProperty1 = "ABC" };
            var a2 = new A { TestProperty1 = "BCD" };
            var comparer = new ObjectsDataComparer<A>();
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.AddComparerOverride(() => a1.TestProperty1, valueComparer);

            var differences = comparer.Compare(a1, a2);

            CollectionAssert.IsEmpty(differences);
            valueComparer.Received(1).Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideBClassComparerEqualTest()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7898" } };
            var valueComparer = CreateClassBComparerAsPhone();
            var comparer = new ObjectsDataComparer<A>();
            comparer.AddComparerOverride<B>(valueComparer);

            var differences = comparer.Compare(a1, a2);

            CollectionAssert.IsEmpty(differences);
        }

        [Test]
        public void OverrideBClassComparerNotEqualTest()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var valueComparer = CreateClassBComparerAsPhone();
            var comparer = new ObjectsDataComparer<A>();
            comparer.AddComparerOverride<B>(valueComparer);

            var differences = comparer.Compare(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB", differences[0].MemberPath);
            Assert.AreEqual("123-456-7898", differences[0].Value1);
            Assert.AreEqual("(123)-456-7899", differences[0].Value2);
        }

        [Test]
        public void OverrideBClassProperty1ComparerEqualTest()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7898" } };
            var valueComparer = CreatePhoneComparer();
            var comparer = new ObjectsDataComparer<A>();
            comparer.AddComparerOverride(() => new B().Property1, valueComparer);

            var differences = comparer.Compare(a1, a2);

            CollectionAssert.IsEmpty(differences);
        }

        [Test]
        public void OverrideBClassProperty1ComparerNotEqualTest()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var valueComparer = CreatePhoneComparer();
            var comparer = new ObjectsDataComparer<A>();
            comparer.AddComparerOverride(() => new B().Property1, valueComparer);

            var differences = comparer.Compare(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB.Property1", differences[0].MemberPath);
            Assert.AreEqual("123-456-7898", differences[0].Value1);
            Assert.AreEqual("(123)-456-7899", differences[0].Value2);
        }

        private IValueComparer CreateClassBComparerAsPhone()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(callInfo =>
            {
                var b1 = (B)callInfo.ArgAt<object>(0);
                var b2 = (B)callInfo.ArgAt<object>(1);

                return ExtractDigits(b1.Property1) == ExtractDigits(b2.Property1);
            });
            valueComparer.ToString(Arg.Any<object>()).Returns(callInfo => ((B)callInfo.Arg<object>()).Property1);

            return valueComparer;
        }

        private IValueComparer CreatePhoneComparer()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(callInfo =>
            {
                var s1 = (string)callInfo.ArgAt<object>(0);
                var s2 = (string)callInfo.ArgAt<object>(1);

                return ExtractDigits(s1) == ExtractDigits(s2);
            });
            valueComparer.ToString(Arg.Any<object>()).Returns(callInfo => ((string)callInfo.Arg<object>()));

            return valueComparer;
        }

        private string ExtractDigits(string str)
        {
            return string.Join(string.Empty, (str ?? string.Empty).ToCharArray().Where(char.IsDigit));
        }
    }
}
