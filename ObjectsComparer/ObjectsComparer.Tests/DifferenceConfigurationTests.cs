using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests

{
    [TestFixture]
    public class DifferenceConfigurationTests
    {
        [Test]
        public void CreateDifferenceDefaultBehavior()
        {
            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference, null, null);

            Assert.IsTrue(targetDifference.MemberPath == targetDifference.MemberPath);
            Assert.IsTrue(targetDifference.Value1 == targetDifference.Value1);
            Assert.IsTrue(targetDifference.Value2 == targetDifference.Value2);
            Assert.IsTrue(targetDifference.RawValue2 == null);
            Assert.IsTrue(targetDifference.RawValue1 == null);
            Assert.IsTrue(targetDifference.DifferenceType == targetDifference.DifferenceType);
        }

        [Test]
        public void CreateDifferenceIncludeRawValues()
        {
            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(includeRawValues: true);

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference,null, null);

            Assert.IsTrue(targetDifference == sourceDifference);
        }

        [Test]
        public void CreateDifferenceNotIncludeRawValues()
        {
            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(includeRawValues: false);

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference, null, null);

            Assert.IsTrue(targetDifference.MemberPath == targetDifference.MemberPath);
            Assert.IsTrue(targetDifference.Value1 == targetDifference.Value1);
            Assert.IsTrue(targetDifference.Value2 == targetDifference.Value2);
            Assert.IsTrue(targetDifference.RawValue2 == null);
            Assert.IsTrue(targetDifference.RawValue1 == null);
            Assert.IsFalse(targetDifference == sourceDifference);
                         }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NoDifferenceFactorySourceTargetDifferenceEquality(bool includeRawValues)
        {
            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(includeRawValues);

            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference, null, null);

            Assert.AreEqual(includeRawValues, sourceDifference == targetDifference);
            Assert.AreEqual(includeRawValues, targetDifference.RawValue1 != null);
            Assert.AreEqual(includeRawValues, targetDifference.RawValue2 != null);
        }
    }
}

