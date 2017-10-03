using System.Collections.Generic;
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
            _comparer = new Comparer();
            _comparer.AddComparerOverride("ConnectionString", DoNotCompareValueComparer.Instance);
        }

        [Test]
        public void Test()
        {
            var mySettingsJson = LoadJson("MySettings.json");
            var mySettings = JsonConvert.DeserializeObject<ExpandoObject>(mySettingsJson);
            var customer1SettingsJson = LoadJson("Customer1Settings.json");
            var customer1Settings = JsonConvert.DeserializeObject<ExpandoObject>(customer1SettingsJson);

            IEnumerable<Difference> differencesEnum;
            var isEqual = _comparer.Compare(mySettings, customer1Settings, out differencesEnum);
            var differences = differencesEnum.ToList();

            Assert.IsTrue(isEqual);
        }

        private string LoadJson(string fileName)
        {
            var resourceStream = typeof(Example3Tests).GetTypeInfo()
                .Assembly.GetManifestResourceStream("ObjectsComparer.Examples.Example3." + fileName);

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
