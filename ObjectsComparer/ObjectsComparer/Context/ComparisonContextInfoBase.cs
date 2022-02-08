using System;

namespace ObjectsComparer
{
    internal class ComparisonContextInfoBase : IComparisonContextInfo
    {
        readonly IComparisonContext _comparisonContext;

        readonly IComparisonContextInfo _ancestor;

        public ComparisonContextInfoBase(IComparisonContext comparisonContext)
        {
            _comparisonContext = comparisonContext ?? throw new ArgumentNullException(nameof(comparisonContext));
            _ancestor = _comparisonContext.Ancestor == null ? null : new ComparisonContextInfo(_comparisonContext.Ancestor);
        }

        public IComparisonContextMember Member => _comparisonContext.Member;

        public IComparisonContextInfo Ancestor => _ancestor;
    }
}