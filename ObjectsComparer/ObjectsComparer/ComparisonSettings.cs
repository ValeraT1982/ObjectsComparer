using ObjectsComparer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectsComparer
{
    /// <summary>
    /// Configuration for Objects Comparer.
    /// </summary>
    public class ComparisonSettings
    {
        /// <summary>
        /// If true, all members which are not primitive types, do not have custom comparison rule and 
        /// do not implement <see cref="IComparable"/> will be compared as separate objects using the same rules as current objects. True by default.
        /// </summary>
        public bool RecursiveComparison { get; set; }

        /// <summary>
        /// If true, empty <see cref="System.Collections.IEnumerable"/>  and null values will be considered as equal values. False by default.
        /// </summary>
        public bool EmptyAndNullEnumerablesEqual { get; set; }

        /// <summary>
        /// If true and member does not exists, objects comparer will consider that this member is equal to default value of opposite member type. 
        /// Applicable for dynamic types comparison only. False by default.
        /// </summary>
        public bool UseDefaultIfMemberNotExist { get; set; }

        /// <summary>
        /// Attribute type used to specify groups of differences.
        /// </summary>
        public Type GroupNameAttribute { get; set; }

        /// <summary>
        /// Attribute type used to define custom names for members
        /// </summary>
        public Type MemberCustomNameAttribute { get; set; }

        private readonly Dictionary<Tuple<Type, string>, object> _settings = new Dictionary<Tuple<Type, string>, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonSettings" /> class. 
        /// </summary>
        public ComparisonSettings()
        {
            RecursiveComparison = true;
            EmptyAndNullEnumerablesEqual = false;
            UseDefaultIfMemberNotExist = false;
            GroupNameAttribute = typeof(GroupNameAttribute);
            MemberCustomNameAttribute = typeof(MemberCustomNameAttribute);
        }

        /// <summary>
        /// Sets value of custom setting. Could be used to pass parameters to custom value comparers.
        /// </summary>
        /// <typeparam name="T">Setting Type.</typeparam>
        /// <param name="value">Setting Value.</param>
        /// <param name="key">Setting Key.</param>
        public void SetCustomSetting<T>(T value, string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);
            _settings[dictionaryKey] = value;
        }

        /// <summary>
        /// Gets value of custom setting. Could be used in custom value comparers.
        /// </summary>
        /// <typeparam name="T">Setting Type.</typeparam>
        /// <param name="key">Setting Key.</param>
        /// <returns>Setting Value.</returns>
        public T GetCustomSetting<T>(string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);

            if (_settings.TryGetValue(dictionaryKey, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException();
        }
    }
}
