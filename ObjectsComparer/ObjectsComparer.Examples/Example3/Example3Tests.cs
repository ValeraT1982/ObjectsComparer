using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

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
            _comparer.AddComparerOverride("ConnectionString", DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride("Email", DoNotCompareValueComparer.Instance);
            _comparer.AddComparerOverride("Notifications", DoNotCompareValueComparer.Instance);
            var disabledByDefaultComparer = new DefaultValueValueComparer<string>("Disabled", IgnoreCaseStringsValueComparer.Instance);
            _comparer.AddComparerOverride("SmartMode1", disabledByDefaultComparer);
            _comparer.AddComparerOverride("SmartMode2", disabledByDefaultComparer);
            _comparer.AddComparerOverride("SmartMode3", disabledByDefaultComparer);
            var urlComparer = new DynamicValueComparer<string>(
                (url1, url2, settings) => url1.Trim('/').Replace(@"http://", string.Empty) == url2.Trim('/').Replace(@"http://", string.Empty));
            _comparer.AddComparerOverride("SomeUrl", urlComparer);
            _comparer.AddComparerOverride("SomeOtherUrl", urlComparer);
            _comparer.AddComparerOverride("DataCompression", new DefaultValueValueComparer<string>("Off", NulableStringsValueComparer.Instance));
            _comparer.AddComparerOverride("ProcessTaskTimeout", new DefaultValueValueComparer<long>(100, DefaultValueComparer.Instance));
            _comparer.AddComparerOverride("TotalProcessTimeout", new DefaultValueValueComparer<long>(500, DefaultValueComparer.Instance));
        }

        [Test]
        public void Customer1Settings()
        {
            var mySettingsJson = LoadJson("MySettings.json");
            var mySettings = JsonConvert.DeserializeObject<ExpandoObject>(mySettingsJson);
            var customer1SettingsJson = LoadJson("Customer1Settings.json");
            var customer1Settings = JsonConvert.DeserializeObject<ExpandoObject>(customer1SettingsJson);

            var isEqual = _comparer.Compare(mySettings, customer1Settings);
            
            Debug.WriteLine("Configurations MySettings.json and Customer1Settings.json are " + (isEqual ? "equal" : "not equal"));

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Customer2Settings()
        {
            var mySettingsJson = LoadJson("MySettings.json");
            var mySettings = JsonConvert.DeserializeObject<ExpandoObject>(mySettingsJson);
            var customer2SettingsJson = LoadJson("Customer2Settings.json");
            var customer2Settings = JsonConvert.DeserializeObject<ExpandoObject>(customer2SettingsJson);

            IEnumerable<Difference> differencesEnum;
            var isEqual = _comparer.Compare(mySettings, customer2Settings, out differencesEnum);
            var differences = differencesEnum.ToList();

            Debug.WriteLine("Configurations MySettings.json and Customer2Settings.json are " + (isEqual ? "equal" : "not equal"));
            foreach (var difference in differences)
            {
                Debug.WriteLine(difference.ToString());
            }

            Assert.IsFalse(isEqual);
            Assert.AreEqual(8, differences.Count);
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Settings.DataCompression" && d.Value1 == "On" && d.Value2 == "Off"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Settings.SuperModes.SmartMode1" && d.Value1 == "Enabled" && d.Value2 == "Disabled"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Timeouts.ProcessTaskTimeout" && d.Value1 == "100" && d.Value2 == "200"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "BackupSettings.BackupIntervalUnit" && d.Value1 == "Day" && d.Value2 == "Week"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "BackupSettings.BackupInterval" && d.Value1 == "100" && d.Value2 == "2"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.Enabled" && d.Value1 == "True" && d.Value2 == "False"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.MaximumFileSize" && d.Value1 == "20MB" && d.Value2 == "40MB"));
            Assert.IsTrue(differences.Any(d => d.MemberPath == "Logging.Level" && d.Value1 == "ALL" && d.Value2 == "ERROR"));
        }
        //Configurations MySettings.json and Customer2Settings.json are not equal
        //Difference: MemberPath= 'Settings.DataCompression', Value1= 'On', Value2= 'Off'.
        //Difference: MemberPath= 'Settings.SuperModes.SmartMode1', Value1= 'Enabled', Value2= 'Disabled'.
        //Difference: MemberPath= 'Timeouts.ProcessTaskTimeout', Value1= '100', Value2= '200'.
        //Difference: MemberPath= 'BackupSettings.BackupIntervalUnit', Value1= 'Day', Value2= 'Week'.
        //Difference: MemberPath= 'BackupSettings.BackupInterval', Value1= '100', Value2= '2'.
        //Difference: MemberPath= 'Logging.Enabled', Value1= 'True', Value2= 'False'.
        //Difference: MemberPath= 'Logging.MaximumFileSize', Value1= '20MB', Value2= '40MB'.
        //Difference: MemberPath= 'Logging.Level', Value1= 'ALL', Value2= 'ERROR'.

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