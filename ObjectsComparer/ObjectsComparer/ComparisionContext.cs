using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectsComparer
{
    public sealed class ComparisionContext : IComparisionContext
    {
        public static readonly IComparisionContext Undefined = new ComparisionContext();

        private ComparisionContext()
        {
        }

        private ComparisionContext(MemberInfo currentMember)
        {
            Member = currentMember ?? throw new ArgumentNullException(nameof(currentMember));
        }

        /// <summary>
        /// 
        /// </summary>
        public MemberInfo Member { get; }

        public static IComparisionContext Create(MemberInfo currentMember)
        {
            if (currentMember is null)
            {
                throw new ArgumentNullException(nameof(currentMember));
            }

            return new ComparisionContext(currentMember);
        }
    }
}