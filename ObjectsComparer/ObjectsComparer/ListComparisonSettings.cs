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
        internal Action<ComparisonContext, ListConfigurationOptions> ConfigureOptionsAction { get; private set; } = null;

        /// <summary>
        /// Configures list comparison behavior.
        /// </summary>
        /// <param name="configureOptions"></param>
        public void Configure(Action<ComparisonContext, ListConfigurationOptions> configureOptions)
        {
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            ConfigureOptionsAction = configureOptions;
        }
    }
}
