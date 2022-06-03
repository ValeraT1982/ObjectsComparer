using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer
{
    public class DifferenceOptions
    {
        DifferenceOptions()
        {
        }

        /// <summary>
        /// Default options.
        /// </summary>
        internal static DifferenceOptions Default() => new DifferenceOptions();

        public bool RawValuesIncluded { get; private set; } = false;

        public void IncludeRawValues(bool value)
        {
            RawValuesIncluded = value;
        }
    }
}
