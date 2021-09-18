using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ObjectsComparer
{
    public sealed class ComparisonContext
    {
        public static readonly ComparisonContext Undefined = new ComparisonContext();

        readonly List<ComparisonContext> _descendants = new List<ComparisonContext>();

        private ComparisonContext()
        {
        }

        private ComparisonContext(MemberInfo currentMember)
        {
            Member = currentMember;
        }

        /// <summary>
        /// It is always null for root context (start point of the comparison) and always null for list item. List item never has got its member. It only has got the ancestor context - list and that list has got its member.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Ancestor context. For example if current context is "Person.Name" property, ancestor is Person.
        /// </summary>
        public ComparisonContext Ancestor { get; set; }

        /// <summary>
        /// Children contexts. For example, if Person class has got properties Name and Birthday, person context has got one child context Name a and one child context Birthday.
        /// </summary>
        public ReadOnlyCollection<ComparisonContext> Descendants => _descendants.AsReadOnly();

        internal static ComparisonContext Create(MemberInfo currentMember = null, ComparisonContext ancestor = null)
        {
            var context = new ComparisonContext(currentMember);
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