using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer
{
    /// <summary>
    /// Options for <see cref="ComparisonSettings.ConfigureDifference(Action{IDifferenceTreeNode, DifferenceOptions})"/> operation.
    /// </summary>
    public class DifferenceOptions
    {
        DifferenceOptions()
        {
        }

        /// <summary>
        /// Default options.
        /// </summary>
        internal static DifferenceOptions Default() => new DifferenceOptions();

        public bool? RawValuesIncluded { get; private set; } = false;

        /// <summary>
        /// Whether the <see cref="Difference"/> instance should contain raw values.
        /// </summary>
        public DifferenceOptions IncludeRawValues(bool value)
        {
            RawValuesIncluded = value;

            return this;
        }

        internal Func<Difference, Difference> DifferenceFactory = null;

        /// <summary>
        /// Factory for <see cref="Difference"/> instances. This takes precendence over <see cref="IncludeRawValues(bool)"/>.
        /// </summary>
        /// <param name="factory">
        /// First parameter: The source difference.<br/>
        /// Returns: Transformed difference or the source difference itself.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DifferenceOptions UseDifferenceFactory(Func<Difference, Difference> factory)
        {
            DifferenceFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            RawValuesIncluded = null;

            return this;
        }

    }
}
