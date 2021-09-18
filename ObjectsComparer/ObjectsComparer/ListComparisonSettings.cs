using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer
{
    public class ListComparisonSettings
    {
        internal Action<ComparisonContext, ListConfigurationOptions> ConfigureOptions { get; private set; } = null;

        public void Configure(Action<ComparisonContext, ListConfigurationOptions> configureOptions)
        {
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            ConfigureOptions = configureOptions;
        }
    }
}
