using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class ListElementComparisonContextMember : IComparisonContextMember
    {
        ListElementComparisonContextMember(MemberInfoComparisonContextMember listMember)
        {
            ListMember = listMember ?? throw new ArgumentNullException(nameof(listMember));
        }

        public MemberInfoComparisonContextMember ListMember { get; }

        public string Name => null;

        public MemberInfo Member => null;
    }
}