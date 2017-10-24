using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ObjectsComparer.Examples
{
    public static class OutputHelper
    {
        public static void ResultToOutput(bool isEqual, IEnumerable<Difference> differenses)
        {
            Debug.WriteLine(isEqual ? "Objects are equal" : string.Join(Environment.NewLine, differenses));
        }
    }
}
