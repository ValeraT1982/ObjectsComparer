using System.Collections.Generic;

namespace ObjectsComparer.Examples.Example4
{
    public class Formula
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IList<FormulaItem> Items { get; set; }
    }
}
