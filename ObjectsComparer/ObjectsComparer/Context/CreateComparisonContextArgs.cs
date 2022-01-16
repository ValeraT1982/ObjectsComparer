using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Arguments for <see cref="IComparisonContextFactory.CreateContext(CreateComparisonContextArgs)"/>.
    /// </summary>
    public class CreateComparisonContextArgs
    {
        public CreateComparisonContextArgs(IComparisonContext ancestor = null, MemberInfo member = null, string memberName = null)
        {
            Ancestor = ancestor;
            Member = member;
            MemberName = memberName;
        }

        public IComparisonContext Ancestor { get; }

        public MemberInfo Member { get; set; }

        public string MemberName { get; set; }
    }
}
