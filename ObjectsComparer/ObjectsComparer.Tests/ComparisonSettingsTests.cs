using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

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

            settings.ConfigureList((comparisonContext, listOptions) =>
            {
                listOptions.CompareUnequalLists(true);

                listOptions.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.UseKey("Key");
                    keyOptions.ThrowKeyNotFound(false);
                });
            });

            //Component side.
            var listConfigurationOptions = ListConfigurationOptions.Default();
            var ctx = ComparisonContext.Create();
            settings.ConfigureOptionsAction(ctx, listConfigurationOptions);
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
        public void FluentTest_CompareUnequalLists()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions.CompareUnequalLists(true));

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(4, differences.Count);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[0].DifferenceType);
            Assert.AreEqual("[0]", differences[0].MemberPath);
            Assert.AreEqual("3", differences[0].Value1);
            Assert.AreEqual("1", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("[2]", differences[1].MemberPath);
            Assert.AreEqual("1", differences[1].Value1);
            Assert.AreEqual("3", differences[1].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[2].DifferenceType);
            Assert.AreEqual("[3]", differences[2].MemberPath);
            Assert.AreEqual("", differences[2].Value1);
            Assert.AreEqual("4", differences[2].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[3].DifferenceType);
            Assert.AreEqual("Length", differences[3].MemberPath);
            Assert.AreEqual("3", differences[3].Value1);
            Assert.AreEqual("4", differences[3].Value2);
        }

        [Test]
        public void FluentTest_CompareUnequalLists_CompareElementsByKey()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("[4]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("Length", differences[1].MemberPath);
            Assert.AreEqual("3", differences[1].Value1);
            Assert.AreEqual("4", differences[1].Value2);
        }

        [Test]
        public void FluentTest_CompareUnequalLists_CompareElementsByKey_FormatKey()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(args => $"Key={args.ElementKey}")));

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("[Key=4]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("Length", differences[1].MemberPath);
            Assert.AreEqual("3", differences[1].Value1);
            Assert.AreEqual("4", differences[1].Value2);
        }

        [Test]
        public void FluentTest_CompareUnequalLists_CompareElementsByKey_FormatKey_DefaultNullElementIdentifier()
        {
            var a1 = new int?[] { 3, 2, 1 };
            var a2 = new int?[] { 1, 2, 3, 4, null };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions.FormatElementKey(args => $"Key={args.ElementKey}")));

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(3, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("[Key=4]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[NullAtIdx=4]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[2].DifferenceType);
            Assert.AreEqual("Length", differences[2].MemberPath);
            Assert.AreEqual("3", differences[2].Value1);
            Assert.AreEqual("5", differences[2].Value2);
        }

        [Test]
        public void FluentTest_CompareUnequalLists_CompareElementsByKey_FormatKey_FormatNullElementIdentifier()
        {
            var a1 = new int?[] { 3, 2, 1 };
            var a2 = new int?[] { 1, 2, 3, 4, null };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions
                    .FormatElementKey(formatArgs => $"Key={formatArgs.ElementKey}")
                    .FormatNullElementIdentifier(formatArgs => $"Null at {formatArgs.ElementIndex}")));

            var comparer = new Comparer(settings);

            //settings = new ComparisonSettings();
            //settings.ConfigureList(listOptions => 
            //{
            //    listOptions.CompareUnequalLists(true);

            //    listOptions.CompareElementsByKey(keyOptions =>
            //    {
            //        keyOptions.FormatElementKey(args => $"Key={args.ElementKey}");
            //        keyOptions.FormatNullElementIdentifier(idx => $"Null at {idx}");
            //    });
            //});

            //settings = new ComparisonSettings();
            //settings.ConfigureList((ctx, listOptions) =>
            //{
            //    bool unequalEnabled = ctx.Member.Name == "List1";
            //    listOptions.CompareUnequalLists(unequalEnabled);

            //    listOptions.CompareElementsByKey(keyOptions =>
            //    {
            //        keyOptions.FormatElementKey(args => $"Key={args.ElementKey}");
            //        keyOptions.FormatNullElementIdentifier(args => $"Null at {args.ElementIndex}");

            //        if (ctx.Member.Name == nameof(A.ListOfB))
            //        {
            //            keyOptions.UseKey(args =>
            //            {
            //                if (args.Element is B element)
            //                {
            //                    return element.Property1;
            //                }

            //                return args.Element;
            //            });
            //        }
            //    });
            //});
                        
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(3, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("[Key=4]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[Null at 4]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("", differences[1].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[2].DifferenceType);
            Assert.AreEqual("Length", differences[2].MemberPath);
            Assert.AreEqual("3", differences[2].Value1);
            Assert.AreEqual("5", differences[2].Value2);
        }

        [Test]
        public void FluentTest_List()
        {
            var a1 = new List<int> { 3, 2, 1 };
            var a2 = new List<int> { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureList(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

            var comparer = new Comparer(settings);
            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences[0].DifferenceType);
            Assert.AreEqual("", differences[0].MemberPath);
            Assert.AreEqual("3", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("[4]", differences[1].MemberPath);
            Assert.AreEqual("", differences[1].Value1);
            Assert.AreEqual("4", differences[1].Value2);
        }
    }
}
