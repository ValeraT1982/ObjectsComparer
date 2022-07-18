using System;
using ObjectsComparer.Exceptions;
using ObjectsComparer.DifferenceTreeExtensions;

namespace ObjectsComparer
{
    public class DifferenceTreeOptions
    {
        DifferenceTreeOptions()
        {
        }

        internal static DifferenceTreeOptions Default()
        {
            return new DifferenceTreeOptions();
        }

        internal Func<IDifferenceTreeNodeMember, IDifferenceTreeNode> DifferenceTreeNodeFactory { get; private set; }

        internal Func<IDifferenceTreeNodeMember, IDifferenceTreeNodeMember> DifferenceTreeNodeMemberFactory { get; private set; }

        /// <summary>
        /// Factory for <see cref="IDifferenceTreeNode"/> instances.
        /// </summary>
        /// <param name="factory">
        /// First parameter: The member the tree node is created for. It can be replaced by a custom instance.
        /// </param>
        public void UseDifferenceTreeNodeFactory(Func<IDifferenceTreeNodeMember, IDifferenceTreeNode> factory)
        {
            DifferenceTreeNodeFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Factory for <see cref="IDifferenceTreeNodeMember"/> instances.
        /// </summary>
        /// <param name="factory">
        /// First parameter: Default member, it can be used as a falback from the delegate.
        /// </param>
        public void UseDifferenceTreeNodeMemberFactory(Func<IDifferenceTreeNodeMember, IDifferenceTreeNodeMember> factory)
        {
            DifferenceTreeNodeMemberFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public bool ThrowDifferenceTreeBuilderNotImplementedEnabled { get; private set; } = true;

        /// <summary>
        /// Whether to throw the <see cref="DifferenceTreeBuilderNotImplementedException"/> when the user requires the difference tree but has a comparer that does not implement <see cref="IDifferenceTreeBuilder"/> or <see cref="IDifferenceTreeBuilder{T}"/>.
        /// Default = true.
        /// </summary>
        public DifferenceTreeOptions ThrowDifferenceTreeBuilderNotImplemented(bool value)
        {
            ThrowDifferenceTreeBuilderNotImplementedEnabled = value;

            return this;
        }
    }
}
