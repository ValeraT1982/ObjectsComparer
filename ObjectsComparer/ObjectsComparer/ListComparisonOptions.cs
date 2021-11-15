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
    public class ListComparisonOptions
    {
        ListComparisonOptions()
        {
        }

        /// <summary>
        /// See <see cref="CompareUnequalLists(bool)"/>.
        /// </summary>
        internal bool UnequalListsComparisonEnabled { get; private set; } = false;

        /// <summary>
        /// Whether to compare elements of the lists even if their number differs. Regardless of the <paramref name="value"/>, if lists are unequal, the difference of type <see cref="DifferenceTypes.NumberOfElementsMismatch"/> will always be logged. Default value = false - unequal lists will not be compared.
        /// </summary>
        public ListComparisonOptions CompareUnequalLists(bool value)
        {
            UnequalListsComparisonEnabled = value;

            return this;
        }

        /// <summary>
        /// Default options.
        /// </summary>
        /// <returns></returns>
        internal static ListComparisonOptions Default() => new ListComparisonOptions();

        /// <summary>
        /// Compares list elements by index. Default behavior.
        /// </summary>
        public ListComparisonOptions CompareElementsByIndex()
        {
            KeyOptionsAction = null;

            return this;
        }

        internal Action<ListElementComparisonByKeyOptions> KeyOptionsAction { get; private set; }

        /// <summary>
        /// Compares list elements by key using <see cref="ListElementComparisonByKeyOptions.DefaultElementKeyProviderAction"/> element key provider.
        /// </summary>
        public ListComparisonOptions CompareElementsByKey()
        {
            return CompareElementsByKey(options => { });
        }

        /// <summary>
        /// Compares list elements by key.
        /// </summary>
        /// <param name="keyOptions">List element comparison options</param>
        public ListComparisonOptions CompareElementsByKey(Action<ListElementComparisonByKeyOptions> keyOptions)
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
