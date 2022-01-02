using System;

namespace ObjectsComparer
{
    internal class ListComparisonContextInfo : IListComparisonContextInfo
    {
        readonly ComparisonContext _comparisonContext;

        readonly IListComparisonContextInfo _ancestor;

        public ListComparisonContextInfo(ComparisonContext comparisonContext)
        {
            _comparisonContext = comparisonContext ?? throw new ArgumentNullException(nameof(comparisonContext));
            _ancestor = _comparisonContext.Ancestor == null ? null : new ListComparisonContextInfo(_comparisonContext.Ancestor);
        }

        public IComparisonContextMember Member => _comparisonContext.Member;

        public IComparisonContextInfo Ancestor => _ancestor;
    }
}