using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Useful arguments for null element identifier formatter. See <see cref="ListElementComparisonByKeyOptions.FormatNullElementIdentifier(Func{FormatNullElementIdentifierArgs, string})"/>.
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
