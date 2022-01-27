using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Base class for <see cref="IComparisonContext"/> implementors.
    /// </summary>
    public abstract class ComparisonContextBase : IComparisonContext
    {
        object _shrinkLock = new object();

        readonly List<IComparisonContext> _descendants = new List<IComparisonContext>();

        readonly List<Difference> _differences = new List<Difference>();

        public ComparisonContextBase(IComparisonContextMember member = null, IComparisonContext ancestor = null)
        {
            ancestor?.AddDescendant(this);
            Member = member;
        }

        public IComparisonContext Ancestor { get; set; }

        public IEnumerable<IComparisonContext> Descendants => _descendants.AsReadOnly();

        public IEnumerable<Difference> Differences => _differences.AsReadOnly();
        
        public IComparisonContextMember Member { get; }

        public void AddDescendant(IComparisonContext descendant)
        {
            if (descendant is null)
            {
                throw new ArgumentNullException(nameof(descendant));
            }

            _descendants.Add(descendant);
            descendant.Ancestor = this;
        }

        public virtual void AddDifference(Difference difference)
        {
            if (difference is null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            _differences.Add(difference);
        }

        public IEnumerable<Difference> GetDifferences(bool recursive)
        {
            foreach (var difference in _differences)
            {
                yield return difference;
            }

            if (recursive)
            {
                foreach (var descendant in _descendants)
                {
                    var differences = descendant.GetDifferences(true);
                    foreach (var difference in differences)
                    {
                        yield return difference;
                    }
                }
            }
        }

        public bool HasDifferences(bool recursive)
        {
            return GetDifferences(recursive).Any();
        }
                
        public IComparisonContext Shrink()
        {
            lock (_shrinkLock)
            {
                List<IComparisonContext> removeDescendants = new List<IComparisonContext>();

                _descendants.ForEach(descendantContext =>
                {
                    descendantContext.Shrink();

                    if (descendantContext.HasDifferences(true) == false)
                    {
                        removeDescendants.Add(descendantContext);
                    }
                });

                _descendants.RemoveAll(ctx => removeDescendants.Contains(ctx));
            }

            return this;
        }
    }
}
