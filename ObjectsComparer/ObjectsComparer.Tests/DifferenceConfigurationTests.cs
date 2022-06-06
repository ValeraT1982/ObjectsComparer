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
        public void RawValuesIncludedDefault()
        {
            var differenceOptions = DifferenceOptions.Default();
            Assert.IsFalse(differenceOptions.RawValuesIncluded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void RawValuesIncluded(bool included)
        {
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(options => options.IncludeRawValues(included));

            var differenceOptions = DifferenceOptions.Default();
            settings.DifferenceOptionsAction(null, differenceOptions);

            Assert.AreEqual(included, differenceOptions.RawValuesIncluded);
        }

        [Test]
        [TestCase("Property1", "Property1", true)]
        [TestCase("Property2", "X", false)]
        public void RawValuesIncludedConditionalCorrectlySet(string propertyName, string expectedPropertyName, bool expected)
        {
            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember(name: propertyName));

            var settings = new ComparisonSettings();

            settings.ConfigureDifference((currentProperty, options) =>
            {
                if (currentProperty.Member.Name == expectedPropertyName)
                {
                    options.IncludeRawValues(true);
                }
            });

            var differenceOptions = DifferenceOptions.Default();
            settings.DifferenceOptionsAction(differenceTreeNode, differenceOptions);

            Assert.AreEqual(expected, differenceOptions.RawValuesIncluded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DifferenceFactoryOverIncludeRawValues(bool includeRawValues)
        {
            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(options =>
            {
                options.IncludeRawValues(includeRawValues);
                options.UseDifferenceFactory(defDifference => defDifference);
            });

            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference);

            Assert.AreEqual(sourceDifference, targetDifference);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NoDifferenceFactorySourceTargetDifferenceEquality(bool includeRawValues)
        {
            var differenceTreeNode = new DifferenceTreeNode(new DifferenceTreeNodeMember());
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(options =>
            {
                options.IncludeRawValues(includeRawValues);
            });

            var sourceDifference = new Difference(
                    memberPath: "PathXY",
                    value1: "VALUE1",
                    value2: "VALUE2",
                    rawValue1: new { Value = "VALUE1" },
                    rawValue2: new { Value = "VALUE2" },
                    differenceType: DifferenceTypes.TypeMismatch);

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference);

            Assert.AreEqual(includeRawValues, sourceDifference == targetDifference);
            Assert.AreEqual(includeRawValues, targetDifference.RawValue1 != null);
            Assert.AreEqual(includeRawValues, targetDifference.RawValue2 != null);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IncludeRawValuesShortcutCorrectlySet(bool includeRawValues)
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

            var targetDifference = DifferenceProvider.CreateDifference(settings, differenceTreeNode, sourceDifference);

            Assert.IsTrue(includeRawValues ? targetDifference.RawValue1 != null : targetDifference.RawValue1 == null);
        }
    }
}

