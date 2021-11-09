using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ObjectsComparer
{
    /// <summary>
    /// Configures list comparison behavior.
    /// </summary>
    public class ListConfigurationOptions
    {
        ListConfigurationOptions()
        {
        }

        /// <summary>
        /// See <see cref="WithUnequalLists(bool)"/>.
        /// </summary>
        internal bool UnequalListsComparisonEnabled { get; private set; } = false;

        /// <summary>
        /// Whether to compare elements of the lists even if their number differs. Regardless of the <paramref name="value"/>, the respective difference of type <see cref="DifferenceTypes.NumberOfElementsMismatch"/> will always be logged. Default value = false.
        /// </summary>
        public ListConfigurationOptions WithUnequalLists(bool value)
        {
            UnequalListsComparisonEnabled = value;

            return this;
        }

        internal static ListConfigurationOptions Default() => new ListConfigurationOptions();

        /// <summary>
        /// Compares list elements by index. Default behavior.
        /// </summary>
        public ListConfigurationOptions CompareElementsByIndex()
        {
            KeyOptionsAction = null;

            return this;
        }

        internal Action<CompareListElementsByKeyOptions> KeyOptionsAction { get; private set; }

        /// <summary>
        /// Compares list elements by key.
        /// </summary>
        public ListConfigurationOptions CompareElementsByKey()
        {
            return CompareElementsByKey(options => { });
        }

        /// <summary>
        /// Compares list elements by key.
        /// </summary>
        public ListConfigurationOptions CompareElementsByKey(Action<CompareListElementsByKeyOptions> keyOptions)
        {
            if (keyOptions is null)
            {
                throw new ArgumentNullException(nameof(keyOptions));
            }

            KeyOptionsAction = keyOptions;

            return this;
        }

        /// <summary>
        /// See <see cref="ListElementSearchMode"/>.
        /// </summary>
        internal ListElementSearchMode ElementSearchMode => KeyOptionsAction == null ? ListElementSearchMode.Index : ListElementSearchMode.Key;
    }
}
