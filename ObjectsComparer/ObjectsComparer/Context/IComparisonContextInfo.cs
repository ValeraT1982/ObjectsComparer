using System;

namespace ObjectsComparer
{
    [Obsolete]
    public interface IComparisonContextInfo
    {
        IComparisonContextMember Member { get; }

        IComparisonContextInfo Ancestor { get; }
    }
}