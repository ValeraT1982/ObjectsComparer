using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Useful arguments for null element identifier formatter. See <see cref="CompareListElementsByKeyOptions.FormatNullElementIdentifier(Func{FormatNullElementIdentifierArgs, string})"/>.
    /// </summary>
    public class FormatNullElementIdentifierArgs
    {
        internal FormatNullElementIdentifierArgs(int elementIndex)
        {
            ElementIndex = elementIndex;
        }

        public int ElementIndex { get; }
    }
}
