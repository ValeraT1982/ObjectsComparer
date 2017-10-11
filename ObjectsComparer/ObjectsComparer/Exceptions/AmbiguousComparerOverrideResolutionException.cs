using System;
using System.Reflection;

namespace ObjectsComparer.Exceptions
{
    /// <summary>
    /// Represents errors that occur when Objects Comparer has more than one comparer override which could be used to compare member.
    /// </summary>
    public class AmbiguousComparerOverrideResolutionException: Exception
    {
        /// <summary>
        /// Name of the Member that was a cause of exception
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// MemberInfo of the Member that was a cause of exception
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <summary>
        /// Type that was a cause of exception
        /// </summary>
        public Type Type { get; }

        internal AmbiguousComparerOverrideResolutionException(MemberInfo memberInfo) 
            : base($"Unable to resolve comparer for member {memberInfo.MemberType}. More than one value comparer meet criteria for this member.")
        {
            MemberInfo = memberInfo;
            MemberName = memberInfo.Name;
        }

        internal AmbiguousComparerOverrideResolutionException(string memberName)
            : base($"Unable to resolve comparer for member {memberName}. More than one value comparer meet criteria for this member.")
        {
            MemberName = memberName;
        }

        internal AmbiguousComparerOverrideResolutionException(Type type)
            : base($"Unable to resolve comparer for type {type.GetTypeInfo().FullName}. More than one value comparer meet criteria for this type.")
        {
            Type = type;
        }
    }
}