using System;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Exceptions;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerOverridesCollectionTests
    {
        [Test]
        public void AddComparerByMemberInfoMemberNull()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer((MemberInfo)null, valueComparer));
        }

        [Test]
        public void AddComparerByMemberInfoComparerNull()
        {
            var memberInfo = Substitute.ForPartsOf<MemberInfo>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer(memberInfo, null));
        }

        [Test]
        public void AddComparerByMemberInfoWhenExists()
        {
            var memberInfo = Substitute.ForPartsOf<MemberInfo>();
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(memberInfo, valueComparer);

            Assert.Throws<ValueComparerExistsException>(() => collection.AddComparer(memberInfo, valueComparer));
        }

        [Test]
        public void GetOverrideByMemberInfoComparer()
        {
            var memberInfo1 = Substitute.ForPartsOf<MemberInfo>();
            var valueComparer1 = Substitute.For<IValueComparer>();
            var memberInfo2 = Substitute.ForPartsOf<MemberInfo>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(memberInfo1, valueComparer1);
            collection.AddComparer(memberInfo2, valueComparer2);

            var valueComparer1FromCollection = collection.GetComparer(memberInfo1);
            var valueComparer2FromCollection = collection.GetComparer(memberInfo2);

            Assert.AreEqual(valueComparer1, valueComparer1FromCollection);
            Assert.AreEqual(valueComparer2, valueComparer2FromCollection);
        }

        [Test]
        public void GetComparerByMemberInfoWhenNotExists()
        {
            var memberInfo1 = Substitute.ForPartsOf<PropertyInfo>();
            var valueComparer1 = Substitute.For<IValueComparer>();
            var memberInfo2 = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo2.PropertyType.Returns(typeof(string));
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(memberInfo1, valueComparer1);

            var valueComparer2FromCollection = collection.GetComparer(memberInfo2);

            Assert.IsNull(valueComparer2FromCollection);
        }

        [Test]
        public void AddComparerByTypeTypeNull()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer((Type)null, valueComparer));
        }

        [Test]
        public void AddComparerByTypeComparerNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer(typeof(string), null));
        }

        [Test]
        public void GetOverrideByTypeComparer()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            collection.AddComparer(typeof(string), valueComparer);

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.AreEqual(valueComparer, valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByTypeComparerWhenTwoComparersMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer(typeof(string), valueComparer1);
            collection.AddComparer(typeof(string), valueComparer2, mi => mi.Name == "Prop1");

            Assert.Throws<AmbiguousComparerOverrideResolutionException>(() => collection.GetComparer(memberInfo));
        }

        [Test]
        public void GetOverrideByTypeComparerWhenOneComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer(typeof(string), valueComparer1, mi => mi.Name == "Prop1");
            collection.AddComparer(typeof(string), valueComparer2, mi => mi.Name == "Prop2");

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.AreEqual(valueComparer1, valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByTypeComparerWhenNoneComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer(typeof(string), valueComparer1, mi => false);
            collection.AddComparer(typeof(string), valueComparer2, mi => false);

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.IsNull(valueComparerFromCollection);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void AddComparerByNameNameEmpty(string memberName)
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentException>(() => collection.AddComparer(memberName, valueComparer));
        }

        [Test]
        public void AddComparerByNameComparerNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer("Name", null));
        }

        [Test]
        public void GetOverrideByNameComparer()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer("Prop1", valueComparer);

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.AreEqual(valueComparer, valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByNameComparerWhenTwoComparersMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer("Prop1", valueComparer1);
            collection.AddComparer("Prop1", valueComparer2, mi => true);

            Assert.Throws<AmbiguousComparerOverrideResolutionException>(() => collection.GetComparer(memberInfo));
        }

        [Test]
        public void GetOverrideByNameComparerWhenNoneComparersMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer("Prop1", valueComparer1, mi => false);
            collection.AddComparer("Prop1", valueComparer2, mi => false);

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.IsNull(valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByNameComparerWhenOneComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            memberInfo.Name.Returns("Prop1");
            collection.AddComparer("Prop1", valueComparer1, mi => false);
            collection.AddComparer("Prop1", valueComparer2, mi => true);

            var valueComparerFromCollection = collection.GetComparer(memberInfo);

            Assert.AreEqual(valueComparer2, valueComparerFromCollection);
        }

        [Test]
        public void GetComparerNullMember()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.GetComparer(null));
        }
    }
}
