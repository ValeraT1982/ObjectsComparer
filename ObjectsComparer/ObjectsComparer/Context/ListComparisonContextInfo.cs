using System;

namespace ObjectsComparer
{
    internal class ListComparisonContextInfo : ComparisonContextInfoBase, IListComparisonContextInfo
    {
        public ListComparisonContextInfo(IComparisonContext comparisonContext) : base(comparisonContext)
        {
        }
    }

    internal class ComparisonContextInfo : ComparisonContextInfoBase
    {
        public ComparisonContextInfo(IComparisonContext comparisonContext) : base(comparisonContext)
        {
        }
    }

    internal class ComparisonContextInfoBase : IComparisonContextInfo
    {
        readonly IComparisonContext _comparisonContext;

        readonly IListComparisonContextInfo _ancestor;

        public ComparisonContextInfoBase(IComparisonContext comparisonContext)
        {
            _comparisonContext = comparisonContext ?? throw new ArgumentNullException(nameof(comparisonContext));
            _ancestor = _comparisonContext.Ancestor == null ? null : new ListComparisonContextInfo(_comparisonContext.Ancestor);
        }

        public IComparisonContextMember Member => _comparisonContext.Member;

        public IComparisonContextInfo Ancestor => _ancestor;
    }
}