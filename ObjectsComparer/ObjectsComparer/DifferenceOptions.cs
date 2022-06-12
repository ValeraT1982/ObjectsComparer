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
            //DifferenceFactory = value ? d => d : DefaultDifferenceFactory;

            return this;
        }

        /// <summary>
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        internal Func<CreateDifferenceArgs, Difference> DifferenceFactory { get; private set; } = DefaultDifferenceFactory;

        /// <summary>
        /// Default difference factory.
        /// If the source difference contains raw values it creates a new <see cref="Difference"/> instance based on the source difference without those values. Otherwise, it returns the source difference itself.
        /// </summary>
        public static Func<CreateDifferenceArgs, Difference> DefaultDifferenceFactory => args => args.DefaultDifference;

        /// <summary>
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        /// <param name="factory">
        /// First parameter: The source difference.<br/>
        /// Returns: Transformed difference or the source difference itself.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DifferenceOptions UseDifferenceFactory(Func<CreateDifferenceArgs, Difference> factory)
        {
            DifferenceFactory = factory ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
    public class CreateDifferenceArgs 
    {
        public object RawValue1 { get; }

        public object RawValue2 { get; }
        public Difference DefaultDifference { get; }

        public CreateDifferenceArgs(Difference defaultDifference, object rawValue1 = null, object rawValue2 = null)
        {
            DefaultDifference = defaultDifference ?? throw new ArgumentNullException(nameof(defaultDifference));
            RawValue1 = rawValue1;
            RawValue2 = rawValue2;
        }
    }
}
