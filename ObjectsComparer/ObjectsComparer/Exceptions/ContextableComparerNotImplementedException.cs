using System;

namespace ObjectsComparer.Exceptions
{
    public class ContextableComparerNotImplementedException : NotImplementedException
    {
        internal ContextableComparerNotImplementedException(string message) : base(message)
        {
        }
    }
}
