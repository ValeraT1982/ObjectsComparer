using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Default implementation of <see cref="IDifferenceTreeNode"/>.
    /// </summary>
    public sealed class DifferenceTreeNode : DifferenceTreeNodeBase
    {
        public DifferenceTreeNode(IDifferenceTreeNodeMember member = null, IDifferenceTreeNode ancestor = null) : base(member, ancestor)
        {

        }
    }
}