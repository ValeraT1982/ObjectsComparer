using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer.Utils
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Returns the left part of <paramref name="value"/> with the <paramref name="length"/> of characters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string value, int length)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Substring(0, value.Length > length ? length : value.Length);
        }
    }
}
