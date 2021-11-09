using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparisonSettingsTests
    {
        [Test]
        public void SetgetCustomSettingWithoutKey()
        {
            var settings = new ComparisonSettings();
            settings.SetCustomSetting("test string");

            var settingValue = settings.GetCustomSetting<string>();

            Assert.AreEqual("test string", settingValue);
        }

        [Test]
        public void SetgetCustomSettingWithKey()
        {
            var settings = new ComparisonSettings();
            settings.SetCustomSetting(123, "setting1");
            settings.SetCustomSetting(234, "setting2");

            var setting1Value = settings.GetCustomSetting<int>("setting1");
            var setting2Value = settings.GetCustomSetting<int>("setting2");

            Assert.AreEqual(123, setting1Value);
            Assert.AreEqual(234, setting2Value);
        }

        [Test]
        public void WronkKey()
        {
            var settings = new ComparisonSettings();
            settings.SetCustomSetting(123, "setting1");

            Assert.Throws<KeyNotFoundException>(() => settings.GetCustomSetting<int>("wrongSettingKey"));
        }

        [Test]
        public void WronkType()
        {
            var settings = new ComparisonSettings();
            settings.SetCustomSetting(123, "setting1");

            Assert.Throws<KeyNotFoundException>(() => settings.GetCustomSetting<double>("setting1"));
        }

        /// <summary>
        /// Whether list comparison by key is correctly set.
        /// </summary>
        [Test]
        public void CompareListElementsByKeyIsCorrectlySet()
        {
            //Client side.
            var settings = new ComparisonSettings();

            settings.List.Configure((comparisonContext, listOptions) =>
            {
                listOptions.WithUnequalLists(true);

                listOptions.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.UseKey("Key");
                    keyOptions.ThrowKeyNotFoundEnabled = false;
                });
            });

            //Component side.
            var listConfigurationOptions = ListConfigurationOptions.Default();
            var ctx = ComparisonContext.Create();
            settings.List.ConfigureOptionsAction(ctx, listConfigurationOptions);
            var compareElementsByKeyOptions = CompareListElementsByKeyOptions.Default();
            listConfigurationOptions.KeyOptionsAction(compareElementsByKeyOptions);

            Assert.AreEqual(true, listConfigurationOptions.UnequalListsComparisonEnabled);
            Assert.AreEqual(true, listConfigurationOptions.ElementSearchMode == ListElementSearchMode.Key);
            Assert.AreEqual(false, compareElementsByKeyOptions.ThrowKeyNotFoundEnabled);
        }

        /// <summary>
        /// Whether backward compatibility is ensured i.e. sequential comparing of equal lists.
        /// </summary>
        [Test]
        public void ListComparisonConfigurationBackwardCompatibilityEnsured()
        {
            var options = ListConfigurationOptions.Default();

            Assert.AreEqual(false, options.UnequalListsComparisonEnabled);
            Assert.AreEqual(true, options.ElementSearchMode == ListElementSearchMode.Index);
        }

        [Test]
        public void PrubeznyTest()
        {
            var a1 = new ArrayList() { 1, 2, 3 };
            var a2 = new ArrayList() { 1, 2, 3, 4 };
            var comparer = new Comparer();

            var result = comparer.CalculateDifferences(a1, a2).ToArray();
        }

        [Test]
        public void FluentTest()
        {
            var a1 = new ArrayList() { 3, 2, 1 };
            var a2 = new ArrayList() { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.List.Configure(listOptions => listOptions.WithUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[1]", differences[0].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("4", differences[1].Value2);
        }
    }
}
