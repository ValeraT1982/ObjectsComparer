using System.Linq;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerOverridesTests
    {
        [Test]
        public void OverrideStringComparisonWhenEqual()
        {
            var a1 = new A { TestProperty1 = "ABC", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { TestProperty1 = "BCD", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str2" } };
            var comparer = new Comparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.AddComparerOverride<string>(stringComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideStringComparisonWhenNotEqual()
        {
            var a1 = new A { TestProperty1 = "ABC", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var a2 = new A { TestProperty1 = "BCD", TestProperty2 = "ABC", ClassB = new B { Property1 = "Str1" } };
            var comparer = new Comparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride(typeof(string), stringComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "TestProperty1" && d.Value1 == "ABC" && d.Value2 == "BCD"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "TestProperty2" && d.Value1 == "ABC" && d.Value2 == "ABC"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "ClassB.Property1" && d.Value1 == "Str1" && d.Value2 == "Str1"));
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
            stringComparer.Received().ToString(Arg.Any<object>());
        }

        [Test]
        public void OverrideIntComparisonWhenNotEqual()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };
            var comparer = new Comparer<A>();
            var stringComparer = Substitute.For<IValueComparer>();
            stringComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(false);
            stringComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride<int>(stringComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "IntArray[0]" && d.Value1 == "1" && d.Value2 == "1"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "IntArray[1]" && d.Value1 == "2" && d.Value2 == "3"));
            stringComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
            stringComparer.Received().ToString(Arg.Any<object>());
        }

        [Test]
        public void OverrideIntComparisonWhenEqual()
        {
            var a1 = new A { IntArray = new[] { 1, 2 } };
            var a2 = new A { IntArray = new[] { 1, 3 } };
            var comparer = new Comparer<A>();
            var intComparer = Substitute.For<IValueComparer>();
            intComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            intComparer.ToString(Arg.Any<object>()).Returns(info => (info.Arg<object>() ?? string.Empty).ToString());
            comparer.AddComparerOverride(typeof(int), intComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            intComparer.Received().Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverridePropertyComparisonWhenEqual()
        {
            var a1 = new A { TestProperty1 = "ABC" };
            var a2 = new A { TestProperty1 = "BCD" };
            var comparer = new Comparer<A>();
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.AddComparerOverride(() => a1.TestProperty1, valueComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
            valueComparer.Received(1).Compare(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideBClassComparerWhenEqual()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7898" } };
            var valueComparer = CreateClassBComparerAsPhone();
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride<B>(valueComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void OverrideBClassComparerWhenNotEqual()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var valueComparer = CreateClassBComparerAsPhone();
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride<B>(valueComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB", differences[0].MemberPath);
            Assert.AreEqual("123-456-7898", differences[0].Value1);
            Assert.AreEqual("(123)-456-7899", differences[0].Value2);
        }

        [Test]
        public void OverrideBClassProperty1ComparerWhenEqual()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7898" } };
            var valueComparer = CreatePhoneComparer();
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride(() => new B().Property1, valueComparer);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void OverrideBClassProperty1ComparerWhenNotEqual()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var valueComparer = CreatePhoneComparer();
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride(() => new B().Property1, valueComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB.Property1", differences[0].MemberPath);
            Assert.AreEqual("123-456-7898", differences[0].Value1);
            Assert.AreEqual("(123)-456-7899", differences[0].Value2);
        }

        [Test]
        public void OverrideBClassProperty1ComparerWithFunction()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7898" } };
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride(
                () => new B().Property1,
                (phone1, phone2, parentSettings) => ExtractDigits(phone1) == ExtractDigits(phone2),
                s => s);

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void OverrideBClassProperty1ComparerWithFunctionAndDefaultToString()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7898" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var comparer = new Comparer<A>();
            comparer.AddComparerOverride(
                () => new B().Property1,
                (phone1, phone2, parentSettings) => ExtractDigits(phone1) == ExtractDigits(phone2));

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("ClassB.Property1", differences[0].MemberPath);
            Assert.AreEqual("123-456-7898", differences[0].Value1);
            Assert.AreEqual("(123)-456-7899", differences[0].Value2);
        }

        [Test]
        public void OverrideBClassProperty1ByMemberInfo()
        {
            var a1 = new A { ClassB = new B { Property1 = "S1" } };
            var a2 = new A { ClassB = new B { Property1 = "S2" } };
            var comparer = new Comparer<A>();
            var valueComparer = Substitute.For<IValueComparer>();
            valueComparer.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            comparer.AddComparerOverride(
                typeof(B).GetTypeInfo().GetMember("Property1").First(),
                valueComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsEmpty(differences);
            valueComparer.Received().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideByName()
        {
            var a1 = new A { ClassB = new B { Property1 = "123-456-7899" } };
            var a2 = new A { ClassB = new B { Property1 = "(123)-456-7899" } };
            var comparer = new Comparer<A>();
            var phoneComparer = Substitute.For<IValueComparer>();
            phoneComparer.Compare("123-456-7899", "(123)-456-7899", Arg.Any<ComparisonSettings>()).Returns(true);

            comparer.AddComparerOverride("Property1", phoneComparer);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsEmpty(differences);
            phoneComparer.Received().Compare("123-456-7899", "(123)-456-7899", Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideMemberHighestPriority()
        {
            var a1 = new A { ClassB = new B { Property1 = "S1" } };
            var a2 = new A { ClassB = new B { Property1 = "S2" } };
            var comparer = new Comparer<A>();
            var valueComparer1 = Substitute.For<IValueComparer>();
            valueComparer1.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var valueComparer2 = Substitute.For<IValueComparer>();
            valueComparer2.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var valueComparer3 = Substitute.For<IValueComparer>();
            valueComparer3.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);

            comparer.AddComparerOverride<string>(valueComparer1);
            comparer.AddComparerOverride(() => a1.ClassB.Property1, valueComparer2);
            comparer.AddComparerOverride("Property1", valueComparer3);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsEmpty(differences);
            valueComparer2.Received().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
            valueComparer1.DidNotReceive().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
            valueComparer3.DidNotReceive().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideMemberHigherPriorityThanByName()
        {
            var a1 = new A { ClassB = new B { Property1 = "S1" } };
            var a2 = new A { ClassB = new B { Property1 = "S2" } };
            var comparer = new Comparer<A>();
            var valueComparer1 = Substitute.For<IValueComparer>();
            valueComparer1.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var valueComparer2 = Substitute.For<IValueComparer>();
            valueComparer2.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);

            comparer.AddComparerOverride<string>(valueComparer1);
            comparer.AddComparerOverride(() => a1.ClassB.Property1, valueComparer1);
            comparer.AddComparerOverride("Property1", valueComparer2);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsEmpty(differences);
            valueComparer1.Received().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
            valueComparer2.DidNotReceive().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
        }

        [Test]
        public void OverrideTypeWithFilterAndByName()
        {
            var a1 = new A { ClassB = new B { Property1 = "S1" } };
            var a2 = new A { ClassB = new B { Property1 = "S2" } };
            var comparer = new Comparer<A>();
            var valueComparer1 = Substitute.For<IValueComparer>();
            valueComparer1.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);
            var valueComparer2 = Substitute.For<IValueComparer>();
            valueComparer2.Compare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ComparisonSettings>()).Returns(true);

            comparer.AddComparerOverride<string>(valueComparer1, memberInfo => memberInfo.Name != "Property1");
            comparer.AddComparerOverride("Property1", valueComparer2);

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsEmpty(differences);
            valueComparer1.DidNotReceive().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
            valueComparer2.Received().Compare("S1", "S2", Arg.Any<ComparisonSettings>());
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
            valueComparer.ToString(Arg.Any<object>()).Returns(callInfo => (string)callInfo.Arg<object>());

            return valueComparer;
        }

        private string ExtractDigits(string str)
        {
            return string.Join(string.Empty, (str ?? string.Empty).ToCharArray().Where(char.IsDigit));
        }
    }
}
