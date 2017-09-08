using System;
using System.Reflection;

namespace ObjectsComparer.Exceptions
{
    public class AmbiguousComparerOverrideResolutionException: Exception
    {
        public MemberInfo MemberInfo { get; }

        public AmbiguousComparerOverrideResolutionException(MemberInfo memberInfo) 
            : base($"Unable to resolve comparer for member {memberInfo.MemberType}. More than one value comparer meet criteria of this member.")
        {
            MemberInfo = memberInfo;
        }
    }
}