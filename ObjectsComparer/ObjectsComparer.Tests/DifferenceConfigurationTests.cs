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
        public void RawValuesIncludedConditional(string propertyName, string expectedPropertyName, bool expected)
        {
            var diffNode = new DifferenceTreeNode(new DifferenceTreeNodeMember(name: propertyName));

            var settings = new ComparisonSettings();

            settings.ConfigureDifference((currentProperty, options) => 
            {
                if (currentProperty.Member.Name == expectedPropertyName)
                {
                    options.IncludeRawValues(true);
                }
            });

            var differenceOptions = DifferenceOptions.Default();
            settings.DifferenceOptionsAction(diffNode, differenceOptions);

            Assert.AreEqual(expected, differenceOptions.RawValuesIncluded);
        }
    }
}

