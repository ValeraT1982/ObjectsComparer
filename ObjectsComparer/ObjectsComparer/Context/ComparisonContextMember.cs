using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class ComparisonContextMember : IComparisonContextMember
    {
        public ComparisonContextMember(MemberInfo info = null, string name = null) 
        {
            Info = info;
            Name = name;
        }

        public MemberInfo Info { get; }

        public string Name { get; }
    }
}