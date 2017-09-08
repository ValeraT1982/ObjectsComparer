using System;
using System.Reflection;

namespace ObjectsComparer.Exceptions
{
    public class ValueComparerExistsException: Exception
    {
        public MemberInfo MemberInfo { get; }

        public ValueComparerExistsException(MemberInfo memberInfo) 
            : base($"Comparer override for member {memberInfo.MemberType} has already been added.")
        {
            MemberInfo = memberInfo;
        }
    }
}
