using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using static ObjectsComparer.Examples.OutputHelper;

// ReSharper disable PossibleMultipleEnumeration
namespace ObjectsComparer.Examples.Example3
{
    [TestFixture]
    public class Example3Tests
    {
        private Comparer _comparer;


        [SetUp]
        public void SetUp()
        {
            _comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });
            //Some fields should be ignored
            _comparer.AddComparerOverride("ConnectionString", DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride("Email", DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride("Notifications", DoNotCompareValueComparer.Instance);
            //Smart Modes are disabled by default. These fields are not case sensitive
            var disabledByDefaultComparer = new DefaultValueValueComparer<string>("Disabled", IgnoreCaseStringsValueComparer.Instance);
            _comparer.AddComparerOverride("SmartMode1", disabledByDefaultComparer);
            _comparer.AddComparerOverride("SmartMode2", disabledByDefaultComparer);
            _comparer.AddComparerOverride("SmartMode3", disabledByDefaultComparer);
            //http prefix in URLs should be ignored
            var urlComparer = new DynamicValueComparer<string>(
                (url1, url2, settings) => url1.Trim('/').Replace(@"http://", string.Empty) == url2.Trim('/').Replace(@"http://", string.Empty));
            _comparer.AddComparerOverride("SomeUrl", urlComparer);
            _comparer.AddComparerOverride("SomeOtherUrl", urlComparer);
            //DataCompression is Off by default.
            _comparer.AddComparerOverride("DataCompression", new DefaultValueValueComparer<string>("Off", NulableStringsValueComparer.Instance));
            //ProcessTaskTimeout and TotalProcessTimeout fields have default values.
            _comparer.AddComparerOverride("ProcessTaskTimeout", new DefaultValueValueComparer<long>(100, DefaultValueComparer.Instance));
            _comparer.AddComparerOverride("TotalProcessTimeout", new DefaultValueValueComparer<long>(500, DefaultValueComparer.Instance));
        }

        [Test]
        public void Settings1()
        {
            var settings0Json = LoadJson("Settings0.json");
            var settings0 = JsonConvert.DeserializeObject<ExpandoObject>(settings0Json);
            var settings1Json = LoadJson("Settings1.json");
            var settings1 = JsonConvert.DeserializeObject<ExpandoObject>(settings1Json);

            IEnumerable<Difference> differences;
            var isEqual = _comparer.Compare(settings0, settings1, out differences);

            ResultToOutput(isEqual, differences);
            
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Settings2()
        {
            var settings0Json = LoadJson("Settings0.json");
            var settings0 = JsonConvert.DeserializeObject<ExpandoObject>(settings0Json);
            var settings2Json = LoadJson("Settings2.json");
            var settings2 = JsonConvert.DeserializeObject<ExpandoObject>(settings2Json);

            IEnumerable<Difference> differences;
            var isEqual = _comparer.Compare(settings0, settings2, out differences);

            ResultToOutput(isEqual, differences);

            Assert.IsFalse(isEqual);
            Assert.AreEqual(8, differences.Count());
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Settings.DataCompression" && d.Value1 == "On" && d.Value2 == "Off"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Settings.SuperModes.SmartMode1" && d.Value1 == "Enabled" && d.Value2 == "Disabled"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Timeouts.ProcessTaskTimeout" && d.Value1 == "100" && d.Value2 == "200"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "BackupSettings.BackupIntervalUnit" && d.Value1 == "Day" && d.Value2 == "Week"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "BackupSettings.BackupInterval" && d.Value1 == "100" && d.Value2 == "2"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.Enabled" && d.Value1 == "True" && d.Value2 == "False"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.MaximumFileSize" && d.Value1 == "20MB" && d.Value2 == "40MB"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.Level" && d.Value1 == "ALL" && d.Value2 == "ERROR"));
        }

        private string LoadJson(string fileName)
        {
            var resourceStream = typeof(Example3Tests).GetTypeInfo()
                .Assembly.GetManifestResourceStream("ObjectsComparer.Examples.Example3." + fileName);

            if (resourceStream != null)
            {
                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            throw new Exception($"Resource '{fileName}' not found");
        }
    }
}