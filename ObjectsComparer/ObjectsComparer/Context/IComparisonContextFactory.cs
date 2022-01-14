using System.Reflection;

namespace ObjectsComparer
{
    //public interface IComparisonContextFactory
    //{
    //    IComparisonContext CreateContext(IComparisonContext ancestor = null, string memberName = null);

    //    IComparisonContext CreateContext(IComparisonContext ancestor = null, MemberInfo member = null);
    //}

    public interface IComparisonContextFactory
    {
        IComparisonContext CreateContext(IComparisonContextInfo ancestor = null);

        IComparisonContext CreateContext(string memberName, IComparisonContextInfo ancestor = null);

        IComparisonContext CreateContext(MemberInfo member, IComparisonContextInfo ancestor = null);
    }
}
