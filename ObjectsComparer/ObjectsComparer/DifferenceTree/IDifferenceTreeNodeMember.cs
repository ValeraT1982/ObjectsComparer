using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// The member in the comparison process, usually a property.
    /// </summary>
    public interface IDifferenceTreeNodeMember
    {
        /// <summary>
        /// Member. It should never be empty.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Member. May be null for dynamic properties unknown at compile time.
        /// </summary>
        MemberInfo Info { get; }
    }
}