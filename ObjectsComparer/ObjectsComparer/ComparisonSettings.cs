using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Configuration for Objects Comparer.
    /// </summary>
    public partial class ComparisonSettings
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

        private readonly Dictionary<Tuple<Type, string>, object> _settings = new Dictionary<Tuple<Type, string>, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonSettings" /> class. 
        /// </summary>
        public ComparisonSettings()
        {
            RecursiveComparison = true;
            EmptyAndNullEnumerablesEqual = false;
            UseDefaultIfMemberNotExist = false;
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
                return (T) value;
            }

            throw new KeyNotFoundException();
        }

        internal Action<IComparisonContext, ListComparisonOptions> ListComparisonOptionsAction { get; private set; } = null;

        /// <summary>
        /// Configures list comparison behavior, especially the type of the comparison. For more info, see <see cref="ListComparisonOptions"/>.
        /// </summary>
        /// <param name="comparisonOptions">First parameter: Current list comparison context.</param>
        public ComparisonSettings ConfigureListComparison(Action<IComparisonContext, ListComparisonOptions> comparisonOptions)
        {
            if (comparisonOptions is null)
            {
                throw new ArgumentNullException(nameof(comparisonOptions));
            }

            ListComparisonOptionsAction = comparisonOptions;

            return this;
        }

        /// <summary>
        /// Configures list comparison behavior, especially the type of comparison. For more info, see <see cref="ListComparisonOptions"/>.
        /// </summary>
        /// <param name="comparisonOptions">See <see cref="ListComparisonOptions"/>.</param>
        public void ConfigureListComparison(Action<ListComparisonOptions> comparisonOptions)
        {
            ConfigureListComparison((_, options) => comparisonOptions(options));
        }

        /// <summary>
        /// Configures the type of list comparison and whether to compare unequal lists. For more info, see <see cref="ListComparisonOptions"/>.
        /// </summary>
        /// <param name="compareElementsByKey">
        /// True value is shortcut for <see cref="ListComparisonOptions.CompareElementsByKey()"/> operation.
        /// False value is shortcut for <see cref="ListComparisonOptions.CompareElementsByIndex()"/> operation. Default value = false.
        /// </param>
        /// <param name="compareUnequalLists">
        /// Shortcut for <see cref="ListComparisonOptions.CompareUnequalLists(bool)"/> operation. Default value = false.
        /// </param>
        public void ConfigureListComparison(bool compareElementsByKey = false, bool compareUnequalLists = false)
        {
            ConfigureListComparison(options =>
            {
                options.CompareUnequalLists(compareUnequalLists);

                if (compareElementsByKey)
                {
                    options.CompareElementsByKey();
                }
            });
        }

        internal Action<IComparisonContext, ComparisonContextOptions> ComparisonContextOptionsAction { get; private set; }

        public void ConfigureComparisonContext(Action<IComparisonContext, ComparisonContextOptions> options)
        {
            ComparisonContextOptionsAction = options ?? throw new ArgumentNullException(nameof(options));
        }
    }
}
