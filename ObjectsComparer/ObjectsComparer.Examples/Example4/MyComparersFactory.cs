using System.Collections.Generic;

namespace ObjectsComparer.Examples.Example4
{
    public class MyComparersFactory : ComparersFactory
    {
        public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null,
            BaseComparer parentComparer = null)
        {
            if (typeof(T) != typeof(IList<FormulaItem>))
            {
                return base.GetObjectsComparer<T>(settings, parentComparer);
            }

            var comparer = new CustomFormulaItemsComparer(settings, parentComparer, this);
            
            return (IComparer<T>) comparer;

        }
    }
}