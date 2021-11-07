using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer
{
    /// <summary>
    /// List comparison settings.
    /// </summary>
    public class ListComparisonSettings
    {
        ListComparisonSettings() { }

        internal Action<ComparisonContext, ListConfigurationOptions> ConfigureOptionsAction { get; private set; } = null;

        internal static ListComparisonSettings Default() => new ListComparisonSettings();

        /// <summary>
        /// Configures list comparison behavior, see <see cref="ListConfigurationOptions"/>.
        /// </summary>
        /// <param name="configureOptions">First parameter: Current list comparison context.</param>
        public void Configure(Action<ComparisonContext, ListConfigurationOptions> configureOptions)
        {
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            ConfigureOptionsAction = configureOptions;
        }

        /// <summary>
        /// Configures list comparison behavior, see <see cref="ListConfigurationOptions"/>.
        /// </summary>
        /// <param name="configureOptions">See <see cref="ListConfigurationOptions"/>.</param>
        public void Configure(Action<ListConfigurationOptions> configureOptions)
        {
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            ConfigureOptionsAction = (_, opt) => configureOptions(opt);
        }
    }
}
