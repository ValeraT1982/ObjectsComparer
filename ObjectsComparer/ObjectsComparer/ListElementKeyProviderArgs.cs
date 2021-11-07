using System;

namespace ObjectsComparer
{
    public class ListElementKeyProviderArgs 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">The element whose key is required.</param>
        public ListElementKeyProviderArgs(object element)
        {
            Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <summary>
        /// The element whose key is required.
        /// </summary>
        public object Element { get; }
    }

}
