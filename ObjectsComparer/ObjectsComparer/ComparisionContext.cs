using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectsComparer
{
    public class ComparisionContext
    {
        public static readonly ComparisionContext Undefined = new ComparisionContext();

        private ComparisionContext()
        {
        }

        private ComparisionContext(MemberInfo currentMember)
        {
            CurrentMember = currentMember ?? throw new ArgumentNullException(nameof(currentMember));
        }

        public readonly MemberInfo CurrentMember;

        public static ComparisionContext Create(MemberInfo currentMember)
        {
            return new ComparisionContext(currentMember);
        }
    }
}