using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class MemberInfoComparisonContextMember : IComparisonContextMember
    {
        MemberInfoComparisonContextMember(MemberInfo member)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
        }

        public string Name => Member.Name;

        public MemberInfo Member { get; }

        public static MemberInfoComparisonContextMember Create(MemberInfo member)
        {
            return new MemberInfoComparisonContextMember(member);
        }
    }
}