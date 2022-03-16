using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ObjectsComparer
{
    //TODO: Edit "Once the comparison process is completed, it is possible to traverse "

    /// <summary>
    /// Context of comparison process. Each <see cref="IComparisonContext"/> instance wraps compared <see cref="Member"/>, which is typically property. Each context has its ancestor and descendants the same way as its compared <see cref="Member"/> has its ancestor and descendant members.
    /// Once the comparison process is completed, it is possible to traverse the comparison context graph and see differences at particular members.
    /// </summary>
    public interface IComparisonContext
    {
        /// <summary>
        /// Ancestor context.
        /// </summary>
        IComparisonContext Ancestor { get; set; }

        /// <summary>
        /// Children contexts.
        /// </summary>
        IEnumerable<IComparisonContext> Descendants { get; }

        /// <summary>
        /// A list of differences directly related to the context.
        /// </summary>
        IEnumerable<Difference> Differences { get; }

        /// <summary>
        /// Compared member, for more info see <see cref="IComparisonContextMember"/>.
        /// It should be null for the root context (the starting point of the comparison) and for the list element context. A list element context never has a member, but it has an ancestor context which is the list and that list has its member.
        /// </summary>
        IComparisonContextMember Member { get; }

        /// <summary>
        /// Adds descendant to the context.
        /// </summary>
        /// <param name="descendant"></param>
        void AddDescendant(IComparisonContext descendant);

        /// <summary>
        /// Adds the difference to the context.
        /// </summary>
        /// <param name="difference"></param>
        void AddDifference(Difference difference);

        /// <summary>
        /// Returns differences directly or indirectly related to the context.
        /// </summary>
        /// <param name="recursive">If value is true, it also looks for <see cref="Differences"/> in <see cref="Descendants"/>.</param>
        IEnumerable<Difference> GetDifferences(bool recursive = true);

        /// <summary>
        /// Whether there are differences directly or indirectly related to the context.
        /// </summary>
        /// <param name="recursive">If value is true, it also looks for <see cref="Differences"/> in <see cref="Descendants"/>.</param>
        bool HasDifferences(bool recursive);

        /// <summary>
        /// Removes all <see cref="Descendants"/> which have no <see cref="Differences"/> directly or indirectly in their <see cref="Descendants"/>.
        /// </summary>
        IComparisonContext Shrink();
    }
}
