using System.Reflection;

namespace ObjectsComparer
{
    public interface IComparisonContextFactory
    {
        IComparisonContext CreateContext(IComparisonContext ancestor = null, string memberName = null);

        IComparisonContext CreateContext(IComparisonContext ancestor = null, MemberInfo member = null);
    }

    public class CreateComparisonContextArgs
    {
        internal CreateComparisonContextArgs(IComparisonContext ancestor, string memberName, MemberInfo member)
        {

        }
    }
}
