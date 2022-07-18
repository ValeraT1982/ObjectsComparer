using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Useful arguments for list element key formatter. See <see cref="ListElementComparisonByKeyOptions.FormatElementKey(Func{FormatListElementKeyArgs, string})"/>.
    /// </summary>
    public class FormatListElementKeyArgs
    {
        internal FormatListElementKeyArgs(int elementIndex, object elementKey, object element)
        {
            ElementIndex = elementIndex;
            ElementKey = elementKey ?? throw new ArgumentNullException(nameof(elementKey));
            Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        public int ElementIndex { get; }

        public object ElementKey { get; }

        public object Element { get; }
    }
}
