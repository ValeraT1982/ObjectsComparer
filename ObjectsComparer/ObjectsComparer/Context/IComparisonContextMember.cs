using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Compared member in comparison process, typically the property.
    /// </summary>
    public interface IComparisonContextMember
    {
        /// <summary>
        /// Compared member name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Compared member. May be null for dynamic properties unknown at compile time.
        /// </summary>
        MemberInfo Info { get; }
    }
}