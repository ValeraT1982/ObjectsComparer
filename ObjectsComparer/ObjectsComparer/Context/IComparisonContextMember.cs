using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// The member being compared in the comparison process, usually a property.
    /// </summary>
    public interface IComparisonContextMember
    {
        /// <summary>
        /// Member.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Member. May be null for dynamic properties unknown at compile time.
        /// </summary>
        MemberInfo Info { get; }
    }
}