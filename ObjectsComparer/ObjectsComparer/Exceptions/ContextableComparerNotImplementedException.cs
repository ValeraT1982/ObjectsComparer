using System;

namespace ObjectsComparer.Exceptions
{
    public class ContextableComparerNotImplementedException : NotImplementedException
    {
        internal ContextableComparerNotImplementedException(object comparer) : base(message: $"The comparer argument of type {comparer?.GetType()?.FullName} does not implement IContextableComparer interface.")
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public object Comparer { get; }
    }

    public class ContextableComparerNotImplementedException<T> : NotImplementedException
    {
        internal ContextableComparerNotImplementedException(IComparer<T> comparer) : base(message: $"The comparer argument of type {comparer?.GetType()?.FullName} does not implement IContextableComparer<T> interface.")
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public IComparer<T> Comparer { get; }
    }
}
