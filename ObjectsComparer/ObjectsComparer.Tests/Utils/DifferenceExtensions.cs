using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectsComparer.Tests.Utils
{
    internal class DifferenceEqualityComparer : EqualityComparer<Difference>
    {
        public override bool Equals(Difference b1, Difference b2)
        {
            if (b1 == null && b2 == null)
            {
                return true;
            }
            else if (b1 == null || b2 == null)
            {
                return false;
            }

            return (b1.DifferenceType == b2.DifferenceType && b1.MemberPath == b2.MemberPath && b1.Value1 == b2.Value1 && b1.Value2 == b2.Value2);
        }

        public override int GetHashCode(Difference obj)
        {
            var memberPath = obj.MemberPath ?? string.Empty;
            var value1 = obj.Value1 ?? string.Empty;
            var value2 = obj.Value2 ?? string.Empty;
            var hcode = obj.DifferenceType.GetHashCode() ^ memberPath.GetHashCode() ^ value1.GetHashCode() ^ value2.GetHashCode();

            return hcode.GetHashCode();
        }
    }

    public static class DifferenceExtensions
    {
        public static bool AreEquivalent(this IEnumerable<Difference> diffs1, IEnumerable<Difference> diffs2)
        {
            return diffs1
                .Except(diffs2, new DifferenceEqualityComparer())
                .Any() == false;
        }
    }
}
