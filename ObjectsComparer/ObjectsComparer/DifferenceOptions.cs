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
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        public Func<CreateDifferenceArgs, Difference> DifferenceFactory { get; private set; } = null;

        /// <summary>
        /// Factory for <see cref="Difference"/> instances.
        /// </summary>
        /// <param name="factory">
        /// Null value is allowed here and means that a default behavior of creation of the difference is required.<br/>
        /// First parameter type: The args for the difference creation, see <see cref="CreateDifferenceArgs"/>.<br/>
        /// Returns: Transformed difference or the source difference itself.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DifferenceOptions UseDifferenceFactory(Func<CreateDifferenceArgs, Difference> factory)
        {
            DifferenceFactory = factory;

            return this;
        }

        public DifferenceOptions IncludeRawValues(bool includeRawValues)
        {
            if (includeRawValues)
            {
                UseDifferenceFactory(args => new Difference(
                    args.DefaultDifference.MemberPath,
                    args.DefaultDifference.Value1,
                    args.DefaultDifference.Value2,
                    args.DefaultDifference.DifferenceType,
                    args.RawValue1,
                    args.RawValue2));
            }
            else
            {
                UseDifferenceFactory(null);
            }

            return this;
        }
    }
}
