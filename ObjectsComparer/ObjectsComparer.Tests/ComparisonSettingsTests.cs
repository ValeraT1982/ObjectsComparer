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
    }
}
