using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal abstract class AbstractEnumerablesComparer: AbstractComparer, IComparerWithCondition
    {
        protected AbstractEnumerablesComparer(ComparisonSettings settings, BaseComparer parentComparer,
            IComparersFactory factory)
            : base(settings, parentComparer, factory)
        {
        }

        public bool IsStopComparison(Type type, object obj1, object obj2)
        {
            if (Settings.EmptyAndNullEnumerablesEqual && (obj1 == null || obj2 == null))
            {
                return true;
            }

            return false;
        }

        public virtual bool SkipMember(Type type, MemberInfo member)
        {
            if (type.InheritsFrom(typeof(Array)))
            {
#if NET45
                var array = new int[0];
                if (member.Name == PropertyHelper.GetMemberInfo(() => array.LongLength).Name)
                {
                    return true;
                }
#endif
            }

            return false;
        }

        public abstract override IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);

        public abstract bool IsMatch(Type type, object obj1, object obj2);
    }
}