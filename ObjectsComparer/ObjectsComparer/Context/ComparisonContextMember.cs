using System;
using System.Reflection;

namespace ObjectsComparer
{
    public class ComparisonContextMember : IComparisonContextMember
    {
        ComparisonContextMember() 
        {
        }

        public static ComparisonContextMember Create()
        {
            return new ComparisonContextMember();
        }

        public static ComparisonContextMember Create(MemberInfo member)
        {
            return new ComparisonContextMember { Member = member ?? throw new ArgumentNullException(nameof(member)) };
        }
        public static ComparisonContextMember Create(string memberName)
        {
            return new ComparisonContextMember { MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName)) };
        }

        string _memberName;

        public string MemberName 
        {
            get
            {
                return Member?.Name ?? _memberName;    
            } 
            private set
            {
                _memberName = value;
            }
        }

        public MemberInfo Member { get; private set; }
    }
}