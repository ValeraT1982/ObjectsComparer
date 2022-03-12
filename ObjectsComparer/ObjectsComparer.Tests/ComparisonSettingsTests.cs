using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Tests.Utils;
using System.Reflection;

namespace ObjectsComparer.Tests
{
    //NullAndMissedMemberAreNotEqual_CheckComparisonContext

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

            settings.ConfigureListComparison((curentContext, listOptions) =>
            {
                listOptions.CompareUnequalLists(true);

                listOptions.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.UseKey("Key");
                    keyOptions.ThrowKeyNotFound(false);
                });

                var currentMember = curentContext.Member;
            });

            //Component side.
            var listComparisonOptions = ListComparisonOptions.Default();
            var ctx = new ComparisonContext();
            settings.ListComparisonOptionsAction(ctx, listComparisonOptions);
            var listElementComparisonByKeyOptions = ListElementComparisonByKeyOptions.Default();
            listComparisonOptions.KeyOptionsAction(listElementComparisonByKeyOptions);

            Assert.AreEqual(true, listComparisonOptions.UnequalListsComparisonEnabled);
            Assert.AreEqual(true, listComparisonOptions.ElementSearchMode == ListElementSearchMode.Key);
            Assert.AreEqual(false, listElementComparisonByKeyOptions.ThrowKeyNotFoundEnabled);
        }

        /// <summary>
        /// Whether backward compatibility is ensured i.e. sequential comparing of equal lists.
        /// </summary>
        [Test]
        public void ListComparisonConfigurationBackwardCompatibilityEnsured()
        {
            var options = ListComparisonOptions.Default();

            Assert.AreEqual(false, options.UnequalListsComparisonEnabled);
            Assert.AreEqual(true, options.ElementSearchMode == ListElementSearchMode.Index);
        }

        [Test]
        public void FluentTest_CompareUnequalLists()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareUnequalLists: true);

            var comparer = new Comparer(settings);
            var ctx = new ComparisonContext();
            var differences = comparer.CalculateDifferences(a1.GetType(), a1, a2, ctx).ToList();

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
            
            Assert.IsTrue(differences.AreEquivalent(ctx.GetDifferences(true)));

            var ctxJson = (ctx.Shrink() as ComparisonContext).ToJson();
        }

        [Test]
        public void FluentTest_CompareUnequalLists_ByKey()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareUnequalLists: true, compareElementsByKey: true);

            var comparer = new Comparer(settings);
            var ctx = new ComparisonContext();
            var differences = comparer.CalculateDifferences(a1.GetType(), a1, a2, ctx).ToList();

            Assert.AreEqual(2, differences.Count);

            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[0].DifferenceType);
            Assert.AreEqual("[4]", differences[0].MemberPath);
            Assert.AreEqual("", differences[0].Value1);
            Assert.AreEqual("4", differences[0].Value2);

            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("Length", differences[1].MemberPath);
            Assert.AreEqual("3", differences[1].Value1);
            Assert.AreEqual("4", differences[1].Value2);

            Assert.IsTrue(differences.AreEquivalent(ctx.GetDifferences(true)));
        }

        [Test]
        public void FluentTest_CompareUnequalLists_ByKey_FormatKey()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(listOptions => listOptions
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
            settings.ConfigureListComparison(listOptions => listOptions
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
            settings.ConfigureListComparison(listOptions => listOptions
                .CompareUnequalLists(true)
                .CompareElementsByKey(keyOptions => keyOptions
                    .FormatElementKey(formatArgs => $"Key={formatArgs.ElementKey}")
                    .FormatNullElementIdentifier(formatArgs => $"Null at {formatArgs.ElementIndex}")));

            var comparer = new Comparer(settings);

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
            settings.ConfigureListComparison(listOptions => listOptions.CompareUnequalLists(true).CompareElementsByKey());

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

        [Test]
        public static void LambdaTest()
        {
            var game = new VariableCaptureGame();

            int gameInput = 5;
            game.Run(gameInput);

            int jTry = 10;
            bool result = game.isEqualToCapturedLocalVariable(jTry);
            Console.WriteLine($"Captured local variable is equal to {jTry}: {result}");

            int anotherJ = 3;
            game.updateCapturedLocalVariable(anotherJ);

            bool equalToAnother = game.isEqualToCapturedLocalVariable(anotherJ);
            Console.WriteLine($"Another lambda observes a new value of captured variable: {equalToAnother}");
        }
        // Output:
        // Local variable before lambda invocation: 0
        // 10 is greater than 5: True
        // Local variable after lambda invocation: 10
        // Captured local variable is equal to 10: True
        // 3 is greater than 5: False
        // Another lambda observes a new value of captured variable: True
    }

    public class VariableCaptureGame
    {
        internal Action<int> updateCapturedLocalVariable;
        internal Func<int, bool> isEqualToCapturedLocalVariable;
                

        public void Run(int input)
        {
            int j = 0;

            updateCapturedLocalVariable = x =>
            {
                j = x;
                bool result = j > input;
                Console.WriteLine($"{j} is greater than {input}: {result}");
            };

            isEqualToCapturedLocalVariable = x => x == j;

            Console.WriteLine($"Local variable before lambda invocation: {j}");
            updateCapturedLocalVariable(10);
            Console.WriteLine($"Local variable after lambda invocation: {j}");
        }
    }
}
