using System.Reflection;

namespace ObjectsComparer
{
    public interface IComparisonContextMember
    {
        string Name { get; }

        MemberInfo Member { get; }
    }
}