using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerDynamicObjectsTests
    {
        [Test]
        public void DifferentValues()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            a1.Field3 = true;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = "B";
            a2.Field2 = 8;
            a2.Field3 = false;
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "true" && d.Value2 == "false"));
        }

        [Test]
        public void MissedFields()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new ExpandoObject();
            a2.Field2 = 8;
            a2.Field3 = false;
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(3, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == string.Empty));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == string.Empty && d.Value2 == "false"));
        }

        [Test]
        public void Hierarchy()
        {
            dynamic a1Sub1 = new ExpandoObject();
            a1Sub1.FieldSub1 = 10;
            dynamic a1 = new ExpandoObject();
            a1.FieldSub1 = a1Sub1;
            dynamic a2Sub1 = new ExpandoObject();
            a2Sub1.FieldSub1 = 8;
            dynamic a2 = new ExpandoObject();
            a2.FieldSub1 = a2Sub1;
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(1, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "FieldSub1.Field1" && d.Value1 == "10" && d.Value2 == "8"));
        }

        [Test]
        public void DifferentTypes()
        {
            dynamic a1 = new ExpandoObject();
            a1.Field1 = "A";
            a1.Field2 = 5;
            dynamic a2 = new ExpandoObject();
            a2.Field1 = 5;
            a2.Field2 = "5";
            var comparer = new Comparer();

            IEnumerable<Difference> differencesEnum;
            var isEqual = comparer.Compare(a1, a2, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsFalse(isEqual);
            Assert.AreEqual(2, differences.Count);
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
            Assert.IsTrue(differences.Any(
                d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "5" && d.DifferenceType == DifferenceTypes.TypeMismatch));
        }

        //ToDo: Add tests
        //[Test]
        //public void ComparerOverride()
        //{
        //    dynamic a1 = new ExpandoObject();
        //    a1.Field1 = "A";
        //    dynamic a2 = new ExpandoObject();
        //    a2.Field1 = " A ";

        //    var comparer = new Comparer();
        //    comparer.AddComparerOverride()

        //    IEnumerable<Difference> differencesEnum;
        //    var isEqual = comparer.Compare(a1, a2, out differencesEnum);
        //    var differences = differencesEnum.ToList();

        //    Assert.IsFalse(isEqual);
        //    Assert.AreEqual(3, differences.Count);
        //    Assert.IsTrue(differences.Any(d => d.MemberPath == "Field1" && d.Value1 == "A" && d.Value2 == "B"));
        //    Assert.IsTrue(differences.Any(d => d.MemberPath == "Field2" && d.Value1 == "5" && d.Value2 == "8"));
        //    Assert.IsTrue(differences.Any(d => d.MemberPath == "Field3" && d.Value1 == "true" && d.Value2 == "false"));
        //}
    }
}
