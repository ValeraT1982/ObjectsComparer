using System.Collections.ObjectModel;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Represents the comparison's process context.
    /// </summary>
    public interface IComparisonContext
    {
        /// <summary>
        /// It is always null for root context (start point of the comparison) and always null for list item. List item never has got its member. It only has got the ancestor context - list and that list has got its member.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        /// Ancestor context. For example if current context is "Person.Name" property, ancestor is Person.
        /// </summary>
        IComparisonContext Ancestor { get; }

        /// <summary>
        /// Children contexts. For example, if Person class has got properties Name and Birthday, person context has got one child context Name a and one child context Birthday.
        /// </summary>
        ReadOnlyCollection<IComparisonContext> Descendants { get; }

        //A list of differences directly related to this context.

        //Whether the object has any properties (bool recursive).

        //HasDifferences(bool recursive)
    }
}