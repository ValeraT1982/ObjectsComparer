using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    public class ComparisonSettings
    {
        public bool RecursiveComparison { get; set; }

        public bool EmptyAndNullEnumerablesEqual { get; set; }

        public bool UseDefaultIfMemberNotExist { get; set; }

        private readonly Dictionary<Tuple<Type, string>, object> _settings = new Dictionary<Tuple<Type, string>, object>();

        public ComparisonSettings()
        {
            RecursiveComparison = true;
            EmptyAndNullEnumerablesEqual = false;
            UseDefaultIfMemberNotExist = false;
        }

        public void SetCustomSetting<T>(T value, string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);
            _settings[dictionaryKey] = value;
        }

        public T GetCustomSetting<T>(string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);

            object value;
            if (_settings.TryGetValue(dictionaryKey, out value))
            {
                return (T) value;
            }

            throw new KeyNotFoundException();
        }
    }
}
