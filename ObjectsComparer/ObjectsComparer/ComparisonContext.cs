using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Information about the <see cref="Member"/>, which is typically a property or field, in comparison process. It has its ancestor and descendant contexts in the same way as its member has its ancestor and descendant members in an object graph. It contains all possible member differences.
    /// It is possible to traverse entire compared object graph and see differences at particular members.
    /// </summary>
    public sealed class ComparisonContext
    {
        /// <summary>
        /// Creates comparison context root.
        /// </summary>
        /// <returns></returns>
        public static ComparisonContext CreateRoot() => new ComparisonContext();

        readonly List<ComparisonContext> _descendants = new List<ComparisonContext>();

        private ComparisonContext()
        {
        }

        private ComparisonContext(MemberInfo currentMember)
        {
            Member = currentMember;
        }

        /// <summary>
        /// Typically a property or field in comparison process.
        /// It is always null for the root context (the starting point of the comparison) and always null for the list element. A list element never has a member, but it has an ancestor context which is the list and that list has its member.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Ancestor context.
        /// </summary>
        public ComparisonContext Ancestor { get; set; }

        /// <summary>
        /// Children contexts.
        /// </summary>
        public ReadOnlyCollection<ComparisonContext> Descendants => _descendants.AsReadOnly();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member">See <see cref="Member"/>.</param>
        /// <param name="ancestor">See <see cref="Ancestor"/>.</param>
        /// <returns></returns>
        internal static ComparisonContext Create(MemberInfo member = null, ComparisonContext ancestor = null)
        {
            var context = new ComparisonContext(member);

            if (ancestor != null)
            {
                ancestor.AddDescendant(context);
            }
            
            return context;
        }

        void AddDescendant(ComparisonContext descendant)
        {
            _descendants.Add(descendant);
            descendant.Ancestor = this;
        }

        //A list of differences directly related to this context.

        //Whether the object has any properties (bool recursive).

        //HasDifferences(bool recursive)
    }
}