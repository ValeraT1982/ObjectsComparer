using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer.Examples.Example4
{
    public class CustomFormulaItemsComparer : AbstractComparer<IList<FormulaItem>>
    {
        public CustomFormulaItemsComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(IList<FormulaItem> obj1, IList<FormulaItem> obj2)
        {
            var group = string.Empty;
            if (obj1 == null && obj2 == null)
            {
                yield break;
            }

            if (obj1 == null || obj2 == null)
            {
                yield return new Difference(group, "", DefaultValueComparer.ToString(obj1), DefaultValueComparer.ToString(obj2));
                yield break;
            }

            if (obj1.Count != obj2.Count)
            {
                yield return new Difference(group, "Count", obj1.Count.ToString(), obj2.Count.ToString(), DifferenceTypes.NumberOfElementsMismatch);
            }

            foreach (var formulaItem in obj1)
            {
                var formulaItem2 = obj2.FirstOrDefault(fi => fi.Id == formulaItem.Id);

                if (formulaItem2 != null)
                {
                    var comparer = Factory.GetObjectsComparer<FormulaItem>();

                    foreach (var difference in comparer.CalculateDifferences(formulaItem, formulaItem2))
                    {
                        yield return difference.InsertPath($"[Id={formulaItem.Id}]");
                    }
                }
            }
        }
    }
}
