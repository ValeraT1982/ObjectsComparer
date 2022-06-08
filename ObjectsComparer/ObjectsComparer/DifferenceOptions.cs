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

        /// <summary>
        /// Whether the <see cref="Difference"/> instance should contain raw values.
        /// </summary>
        public DifferenceOptions IncludeRawValues(bool value)
        {
            DifferenceFactory = value ? d => d : DefaultDifferenceFactory;

            return this;
        }

        /// <summary>
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        internal Func<Difference, Difference> DifferenceFactory { get; private set; } = DefaultDifferenceFactory;

        /// <summary>
        /// Default difference factory.
        /// If the source difference contains raw values it creates a new <see cref="Difference"/> instance based on the source difference without those values. Otherwise, it returns the source difference itself.
        /// </summary>
        public static Func<Difference, Difference> DefaultDifferenceFactory => sourceDifference => 
        {
            if (sourceDifference.RawValue1 == null && sourceDifference.RawValue2 == null)
            {
                return sourceDifference;
            }

            return new Difference(
                memberPath: sourceDifference.MemberPath,
                value1: sourceDifference.Value1,
                value2: sourceDifference.Value2,
                differenceType: sourceDifference.DifferenceType);
        };

        /// <summary>
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        /// <param name="factory">
        /// First parameter: The source difference.<br/>
        /// Returns: Transformed difference or the source difference itself.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DifferenceOptions UseDifferenceFactory(Func<Difference, Difference> factory)
        {
            DifferenceFactory = factory ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}
