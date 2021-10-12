using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer.Exceptions
{
    /// <summary>
    /// Depending on the configuration, the exception is thrown if the key provider does not supply the key for list element <see cref="KeylessElement"/>. For more information see <see cref="CompareElementsByKeyOptions.UseKey(Func{object, object})"/> and <see cref="CompareElementsByKeyOptions.ThrowKeyNotFound"/>.
    /// </summary>
    public class ElementKeyNotFoundException : Exception
    {
        const string ElementKeyNotFoundExceptionMsg = "Element key not found.";

        /// <summary>
        /// See <see cref="ElementKeyNotFoundException"/>.
        /// </summary>
        /// <param name="keylessElement">An element that is missing a key.</param>
        /// <param name="message"></param>
        internal ElementKeyNotFoundException(object keylessElement, string message = ElementKeyNotFoundExceptionMsg) : base(message)
        {
            KeylessElement = keylessElement ?? throw new ArgumentNullException(nameof(keylessElement));
        }

        /// <summary>
        /// An element that is missing a key.
        /// </summary>
        public object KeylessElement { get; }
    }
}
