using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Default implementation of <see cref="IComparisonContext"/>.
    /// </summary>
    public sealed class ComparisonContext : ComparisonContextBase
    {
        private ComparisonContext(IComparisonContextMember member = null, IComparisonContext ancestor = null) : base(member, ancestor)
        {

        }
    }
}