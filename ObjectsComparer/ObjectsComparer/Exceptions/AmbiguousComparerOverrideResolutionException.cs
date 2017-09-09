using System;
using System.Reflection;

namespace ObjectsComparer.Exceptions
{
    public class AmbiguousComparerOverrideResolutionException: Exception
    {
        public MemberInfo MemberInfo { get; }

        public Type Type { get; }

        public AmbiguousComparerOverrideResolutionException(MemberInfo memberInfo) 
            : base($"Unable to resolve comparer for member {memberInfo.MemberType}. More than one value comparer meet criteria for this member.")
        {
            MemberInfo = memberInfo;
        }

        public AmbiguousComparerOverrideResolutionException(Type type)
            : base($"Unable to resolve comparer for type {type.GetTypeInfo().FullName}. More than one value comparer meet criteria for this type.")
        {
            Type = type;
        }
    }
}