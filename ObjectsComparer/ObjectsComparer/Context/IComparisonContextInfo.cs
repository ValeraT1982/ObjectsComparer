namespace ObjectsComparer
{
    public interface IComparisonContextInfo
    {
        IComparisonContextMember Member { get; }

        IComparisonContextInfo Ancestor { get; }
    }
}