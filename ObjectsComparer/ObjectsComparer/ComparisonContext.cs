using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Information about the <see cref="Member"/>, which is typically a property or field, in comparison process. It has its ancestor and descendant <see cref="ComparisonContext"/> objects in the same way as its <see cref="Member"/> has its ancestor and descendant members in an object graph. <see cref="ComparisonContext"/> contains all possible member differences.
    /// Once the comparison is completed, it is possible to traverse the object graph and see differences at particular members.
    /// </summary>
    public sealed class ComparisonContext
    {
        object _shrinkLock = new object();

        /// <summary>
        /// Creates comparison context root.
        /// </summary>
        /// <returns></returns>
        public static ComparisonContext CreateRoot() => new ComparisonContext();

        readonly List<ComparisonContext> _descendants = new List<ComparisonContext>();

        readonly List<Difference> _differences = new List<Difference>();

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

        public string MemberName => Member?.Name;

        /// <summary>
        /// Ancestor context.
        /// </summary>
        public ComparisonContext Ancestor { get; private set; }

        /// <summary>
        /// Children contexts.
        /// </summary>
        public ReadOnlyCollection<ComparisonContext> Descendants => _descendants.AsReadOnly();

        /// <summary>
        /// A list of differences directly related to this context.
        /// </summary>
        public ReadOnlyCollection<Difference> Differences => _differences.AsReadOnly();

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

        internal void AddDifference(Difference difference)
        {
            if (difference is null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            _differences.Add(difference);
        }

        /// <summary>
        /// Whether there are differences directly or indirectly related to this context.
        /// </summary>
        /// <param name="recursive">If value is true, it also looks for <see cref="Differences"/> in <see cref="Descendants"/>.</param>
        public bool HasDifferences(bool recursive)
        {
            if (_differences.Any())
            {
                return true;
            }

            if (recursive)
            {
                return _descendants.Any(d => d.HasDifferences(true));
            }

            return false;
        }

        public void Shrink()
        {
            lock (_shrinkLock)
            {
                List<ComparisonContext> removeDescendants = new List<ComparisonContext>();

                _descendants.ForEach(d =>
                {
                    d.Shrink();

                    if (d.HasDifferences(true) == false)
                    {
                        removeDescendants.Add(d);
                    }
                });

                removeDescendants.ForEach(d => _descendants.Remove(d));
            }
            
        }

        //public bool ShouldSerializeMember()
        //{
        //    return false;
        //}
    }
}