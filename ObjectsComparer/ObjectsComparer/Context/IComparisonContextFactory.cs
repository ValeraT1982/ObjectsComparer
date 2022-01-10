using System.Reflection;

namespace ObjectsComparer
{
    public interface IComparisonContextFactory
    {
        IComparisonContext CreateContext(IComparisonContext ancestor = null, MemberInfo member = null, string memberName = null);
    }

    public class CreateComparisonContextArgs
    {
        internal CreateComparisonContextArgs(IComparisonContext ancestor, string memberName, MemberInfo member)
        {

        }
    }
}
