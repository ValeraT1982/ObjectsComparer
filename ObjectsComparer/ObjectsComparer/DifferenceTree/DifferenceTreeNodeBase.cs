using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    /// <summary>
    /// Base class for <see cref="IDifferenceTreeNode"/> implementors.
    /// </summary>
    public abstract class DifferenceTreeNodeBase : IDifferenceTreeNode
    {
        readonly List<IDifferenceTreeNode> _descendants = new List<IDifferenceTreeNode>();

        readonly List<Difference> _differences = new List<Difference>();

        public DifferenceTreeNodeBase(IDifferenceTreeNodeMember member = null, IDifferenceTreeNode ancestor = null)
        {
            ancestor?.AddDescendant(this);
            Member = member;
        }

        IDifferenceTreeNode _ancestor;

        public virtual IDifferenceTreeNode Ancestor
        {
            get
            {
                return _ancestor; 
            }
            set
            {
                if (_ancestor != null)
                {
                    throw new InvalidOperationException("The ancestor already exists.");
                }

                _ancestor = value;
            } 
        }

        public IEnumerable<IDifferenceTreeNode> Descendants => _descendants.AsReadOnly();

        public IEnumerable<Difference> Differences => _differences.AsReadOnly();
        
        public IDifferenceTreeNodeMember Member { get; }

        public void AddDescendant(IDifferenceTreeNode descendant)
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
                
        public void Shrink()
        {
            List<IDifferenceTreeNode> removeDescendants = new List<IDifferenceTreeNode>();

            _descendants.ForEach(descendantNode =>
            {
                descendantNode.Shrink();

                if (descendantNode.HasDifferences(true) == false)
                {
                    removeDescendants.Add(descendantNode);
                }
            });

            _descendants.RemoveAll(descendantTreeNode  => removeDescendants.Contains(descendantTreeNode ));
        }
    }
}
