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
        /// Whether to compare elements of the lists even if their number differs. Regardless of the value, respective difference of type <see cref="DifferenceTypes.NumberOfElementsMismatch"/> will always be logged. Default value = false.
        /// </summary>
        public bool CompareUnequalLists { get; set; } = false;

        internal static ListConfigurationOptions Default() => new ListConfigurationOptions();

        /// <summary>
        /// Compares list elements by index. Default behavior.
        /// </summary>
        public void CompareElementsByIndex()
        {
            KeyOptionsAction = null;
        }

        internal Action<CompareElementsByKeyOptions> KeyOptionsAction { get; private set; }

        /// <summary>
        /// Compares list elements by key.
        /// </summary>
        public void CompareElementsByKey()
        {
            CompareElementsByKey(options => { });
        }

        /// <summary>
        /// Compares list elements by key.
        /// </summary>
        public void CompareElementsByKey(Action<CompareElementsByKeyOptions> keyOptions)
        {
            if (keyOptions is null)
            {
                throw new ArgumentNullException(nameof(keyOptions));
            }

            KeyOptionsAction = keyOptions;
        }

        /// <summary>
        /// See <see cref="ListElementSearchMode"/>.
        /// </summary>
        internal ListElementSearchMode ElementSearchMode => KeyOptionsAction == null ? ListElementSearchMode.Index : ListElementSearchMode.Key;
    }
}
