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
        public void AddComparerByMemberInfoWhenMemberNull()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            // ReSharper disable once RedundantCast
            Assert.Throws<ArgumentNullException>(() => collection.AddComparer((MemberInfo)null, valueComparer));
        }

        [Test]
        public void AddComparerByMemberInfoWhenComparerNull()
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

            var exception = Assert.Throws<ValueComparerExistsException>(() => collection.AddComparer(memberInfo, valueComparer));
            Assert.AreEqual(memberInfo, exception.MemberInfo);
        }

        [Test]
        public void GetOverrideByMemberInfoComparer()
        {
            var memberInfo1 = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo1.PropertyType.Returns(typeof(string));
            var valueComparer1 = Substitute.For<IValueComparer>();
            var memberInfo2 = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo2.PropertyType.Returns(typeof(string));
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
            memberInfo1.PropertyType.Returns(typeof(string));
            var valueComparer1 = Substitute.For<IValueComparer>();
            var memberInfo2 = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo2.PropertyType.Returns(typeof(string));
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(memberInfo1, valueComparer1);

            var valueComparer2FromCollection = collection.GetComparer(memberInfo2);

            Assert.IsNull(valueComparer2FromCollection);
        }

        [Test]
        public void AddComparerByTypeWhenTypeNull()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer((Type)null, valueComparer));
        }

        [Test]
        public void AddComparerByTypeWhenComparerNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer(typeof(string), null));
        }

        [Test]
        public void GetOverrideByTypeComparerByMemberInfo()
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
        public void GetOverrideByTypeComparerByType()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            var memberInfo = Substitute.ForPartsOf<PropertyInfo>();
            memberInfo.PropertyType.Returns(typeof(string));
            collection.AddComparer(typeof(string), valueComparer);

            var valueComparerFromCollection = collection.GetComparer(typeof(string));

            Assert.AreEqual(valueComparer, valueComparerFromCollection);
        }

        [Test]
        public void GetComparerByTypeWhenNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.GetComparer((Type)null));
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

            var exception = Assert.Throws<AmbiguousComparerOverrideResolutionException>(() => collection.GetComparer(memberInfo));
            Assert.AreEqual(memberInfo, exception.MemberInfo);
            Assert.AreEqual("Prop1", exception.MemberName);
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
        public void GetOverrideByTypeComparerWhenNoComparerMatchCriteria()
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

        [Test]
        public void GetOverrideByTypeWhenOnlyOneComparerDoNotHaveFilter()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(typeof(string), valueComparer1, mi => true);
            collection.AddComparer(typeof(string), valueComparer2);

            var valueComparerFromCollection = collection.GetComparer(typeof(string));

            Assert.AreEqual(valueComparer2, valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByTypeWhenMoreThanOneComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(typeof(string), valueComparer1);
            collection.AddComparer(typeof(string), valueComparer2);

            var exception = Assert.Throws<AmbiguousComparerOverrideResolutionException>(() => collection.GetComparer(typeof(string)));
            Assert.AreEqual(typeof(string), exception.Type);
        }

        [Test]
        public void GetOverrideByTypeWhenNoComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(typeof(string), valueComparer1);

            var valueComparerFromCollection = collection.GetComparer(typeof(int));

            Assert.IsNull(valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByTypeWhenAllComparersHaveFilters()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer(typeof(string), valueComparer1, mi => true);
            collection.AddComparer(typeof(string), valueComparer2, mi => true);

            var valueComparerFromCollection = collection.GetComparer(typeof(string));

            Assert.IsNull(valueComparerFromCollection);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void AddComparerByNameWhenNameEmpty(string memberName)
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentException>(() => collection.AddComparer(memberName, valueComparer));
        }

        [Test]
        public void AddComparerByNameWhenComparerNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddComparer("Name", null));
        }

        [Test]
        public void GetOverrideByNameComparerByMemberInfo()
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
        public void GetOverrideByNameComparerByMemberInfoWhenTwoComparersMatchCriteria()
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
        public void GetOverrideByNameComparerByMemberInfoWhenNoComparerMatchCriteria()
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
        public void GetOverrideByNameComparerByMemberInfoWhenOneComparerMatchCriteria()
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
        public void GetComparerByMemberInfoWhenNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.GetComparer((MemberInfo)null));
        }

        [Test]
        public void GetOverrideByNameComparerByMemberName()
        {
            var valueComparer = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer("Prop1", valueComparer);

            var valueComparerFromCollection = collection.GetComparer("Prop1");

            Assert.AreEqual(valueComparer, valueComparerFromCollection);
        }

        [Test]
        public void GetOverrideByNameComparerByMemberNameWhenTwoComparersMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer("Prop1", valueComparer1);
            collection.AddComparer("Prop1", valueComparer2);

            Assert.Throws<AmbiguousComparerOverrideResolutionException>(() => collection.GetComparer("Prop1"));
        }

        [Test]
        public void GetOverrideByNameComparerByMemberNameWhenNoComparerMatchCriteria()
        {
            var valueComparer1 = Substitute.For<IValueComparer>();
            var valueComparer2 = Substitute.For<IValueComparer>();
            var collection = new ComparerOverridesCollection();
            collection.AddComparer("Prop1", valueComparer1, mi => false);
            collection.AddComparer("Prop1", valueComparer2, mi => false);

            var valueComparerFromCollection = collection.GetComparer("Prop1");

            Assert.IsNull(valueComparerFromCollection);
        }

        [Test]
        public void GetComparerByMemberNameWhenNull()
        {
            var collection = new ComparerOverridesCollection();

            Assert.Throws<ArgumentNullException>(() => collection.GetComparer((string)null));
        }
    }
}
