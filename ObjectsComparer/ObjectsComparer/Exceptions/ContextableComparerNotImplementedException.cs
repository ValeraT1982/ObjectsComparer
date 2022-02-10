using System;

namespace ObjectsComparer.Exceptions
{
    /// <summary>
    /// Depending on the configuration, this exception may be thrown when a user defined comparer does not implement <see cref="IContextableComparer"/> or <see cref="IContextableComparer{T}"/>.
    /// To prevent this exception from being thrown, see <see cref="ComparisonContextOptions.ThrowContextableComparerNotImplemented(bool)"/> operation.
    /// </summary>
    public class ContextableComparerNotImplementedException : NotImplementedException
    {
        internal ContextableComparerNotImplementedException(string message) : base(message)
        {
        }
    }
}
