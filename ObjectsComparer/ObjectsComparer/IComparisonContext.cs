using System.Collections.ObjectModel;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Represents the comparison's process context.
    /// </summary>
    public interface IComparisonContext
    {
        MemberInfo Member { get; }

        IComparisonContext Ancestor { get; }

        ReadOnlyCollection<IComparisonContext> Descendants { get; }
    }
}