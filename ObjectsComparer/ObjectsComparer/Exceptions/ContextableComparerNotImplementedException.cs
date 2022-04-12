using System;
using ObjectsComparer.DifferenceTreeExtensions;

namespace ObjectsComparer.Exceptions
{
    /// <summary>
    /// Depending on the configuration or actual state of the comparison, this exception may be thrown when a user defined comparer does not implement <see cref="IDifferenceTreeBuilder"/> or <see cref="IDifferenceTreeBuilder{T}"/>.
    /// To prevent this exception from being thrown, see <see cref="ComparisonContextOptions.ThrowContextableComparerNotImplemented(bool)"/> operation.
    /// </summary>
    public class DifferenceTreeBuilderNotImplementedException : NotImplementedException
    {
        internal DifferenceTreeBuilderNotImplementedException(string message) : base(message)
        {
        }
    }
}
