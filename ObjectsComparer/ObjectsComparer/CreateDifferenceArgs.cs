using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Arguments for the <see cref="Difference"/> factory.
    /// </summary>
    public class CreateDifferenceArgs 
    {
        public object RawValue1 { get; }

        public object RawValue2 { get; }

        /// <summary>
        /// The default difference that a factory can return as its fallback.
        /// </summary>
        public Difference DefaultDifference { get; }

        public CreateDifferenceArgs(Difference defaultDifference, object rawValue1 = null, object rawValue2 = null)
        {
            DefaultDifference = defaultDifference ?? throw new ArgumentNullException(nameof(defaultDifference));
            RawValue1 = rawValue1;
            RawValue2 = rawValue2;
        }
    }
}
