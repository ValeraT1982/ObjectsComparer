using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class ListElementComparisonContextMember : IComparisonContextMember
    {
        public ListElementComparisonContextMember(ListComparisonContextMember listMember)
        {
            ListMember = listMember ?? throw new ArgumentNullException(nameof(listMember));
        }

        public ListComparisonContextMember ListMember { get; }

        public string Name => null;

        public MemberInfo Info => null;
    }
}