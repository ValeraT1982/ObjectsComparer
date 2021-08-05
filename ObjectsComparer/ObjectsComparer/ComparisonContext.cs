using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ObjectsComparer
{
    public sealed class ComparisonContext : IComparisonContext
    {
        public static readonly IComparisonContext Undefined = new ComparisonContext();

        readonly List<IComparisonContext> _descendants = new List<IComparisonContext>();

        private ComparisonContext()
        {
        }

        private ComparisonContext(MemberInfo currentMember)
        {
            //Member = currentMember ?? throw new ArgumentNullException(nameof(currentMember));
            Member = currentMember;
        }

        /// <summary>
        /// 
        /// </summary>
        public MemberInfo Member { get; }

        public IComparisonContext Ancestor { get; set; }

        public ReadOnlyCollection<IComparisonContext> Descendants => _descendants.AsReadOnly();

        internal static IComparisonContext Create(MemberInfo currentMember = null, IComparisonContext ancestor = null)
        {
            //if (currentMember is null)
            //{
            //    throw new ArgumentNullException(nameof(currentMember));
            //}

            var context = new ComparisonContext(currentMember);

            if (ancestor != null)
            {
                ((ComparisonContext)ancestor).AddDescendant(context);
            }

            return context;
        }

        void AddDescendant(ComparisonContext descendant)
        {
            _descendants.Add(descendant);
            descendant.Ancestor = this;
        }
    }
}