using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class MemberInfoComparisonContextMember : IComparisonContextMember
    {
        public MemberInfoComparisonContextMember(MemberInfo member)
        {
            Info = member ?? throw new ArgumentNullException(nameof(member));
        }

        public virtual string Name => Info.Name;

        public MemberInfo Info { get; }
    }
}