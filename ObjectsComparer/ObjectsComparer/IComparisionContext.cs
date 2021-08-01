using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Represents the current context of the comparison.
    /// </summary>
    public interface IComparisionContext
    {
        MemberInfo Member { get; }
    }
}