using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class DifferenceTreeNodeMember : IDifferenceTreeNodeMember
    {
        public DifferenceTreeNodeMember(MemberInfo info = null, string name = null) 
        {
            Info = info;
            Name = name;
        }

        public MemberInfo Info { get; }

        public string Name { get; }
    }
}