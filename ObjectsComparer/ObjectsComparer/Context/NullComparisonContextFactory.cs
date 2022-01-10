using System.Reflection;

namespace ObjectsComparer
{
    internal class NullComparisonContextFactory : IComparisonContextFactory
    {
        public IComparisonContext CreateContext(IComparisonContext ancestor = null, string memberName = null)
        {
            return new NullComparisonContext(DefaultComparisonContextFactory.CreateMember(memberName, null), ancestor);
        }

        public IComparisonContext CreateContext(IComparisonContext ancestor = null, MemberInfo member = null)
        {
            return new NullComparisonContext(DefaultComparisonContextFactory.CreateMember(null, member), ancestor);
        }
    }
}
