using System;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Configures the behavior of list elements if elements are to be compared by key.
    /// </summary>
    public class CompareElementsByKeyOptions
    {
        /// <summary>
        /// Default identifier template for those elements that refer null value.  
        /// It will be used as part of the <see cref="Difference.MemberPath"/> property.
        /// For example: "Addresses[NULL_74]" that means there is an element at index 74 in the property Addresses that refers null value.
        /// See <see cref="FormatNullElementIdentifier(Func{int, string})"/> for more info.        
        /// </summary>
        public const string DefaultNullElementIdentifierTemplate = "NULL_{0}";

        CompareElementsByKeyOptions()
        {
            Initialize();
        }

        /// <summary>
        /// See <see cref="FormatElementKey(Func{int, object, string})"/>.
        /// </summary>
        internal Func<int, object, string> ElementKeyFormatter { get; private set; }

        internal Func<int, string> NullElementIdentifierFormatter { get; private set; }

        /// <summary>
        /// If value = false and element key will not be found, the element will be excluded from comparison and no difference will be logged. If value = true and element key will not be found, an exception of type <see cref="ElementKeyNotFoundException"/> will be thrown.
        /// Default value = true.
        /// </summary>
        public bool ThrowKeyNotFound { get; set; } = true;

        /// <summary>
        /// If null, the elements should be compared by their index, otherwise by key. Default value = null.
        /// </summary>
        internal Func<object, object> KeyProvider { get; private set; } = null;

        /// <summary>
        /// If the list element refers to a null value, this symbol will eventually be used as a list element identifier in the <see cref="Difference.MemberPath"/> property.
        /// Default value = <see cref="DefaultNullElementIdentifierTemplate"/>.
        /// If you don't want to use it at all, set this property to <see cref="string.Empty"/>.
        /// </summary>
        public string NullElementIdentifier { get; set; } = DefaultNullElementIdentifierTemplate;

        internal static CompareElementsByKeyOptions Default() => new CompareElementsByKeyOptions();

        void Initialize()
        {
            UseKey(new string[] { "Id", "Name" }, caseSensitive: false);
        }

        /// <summary>
        /// Compares list elements by key. It will try to find a property specified by <paramref name="key"/> parameter.
        /// </summary>
        public void UseKey(string key, bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
            }

            UseKey(new string[] { key }, caseSensitive);
        }

        /// <summary>
        /// Compares list elements by key. It will try to find one of the public properties specified by <paramref name="keys"/> parameter, in that order.
        /// </summary>
        public void UseKey(string[] keys, bool caseSensitive = false)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (keys.Any() == false)
            {
                throw new ArgumentException("At least one key is required.", nameof(keys));
            }

            if (keys.Any(key => string.IsNullOrWhiteSpace(key)))
            {
                throw new ArgumentException($"'{nameof(keys)}' cannot contain null or whitespace.", nameof(keys));
            }

            UseKey(element =>
            {
                return GetKeyValue(element, caseSensitive, keys);
            });
        }

        /// <summary>
        /// Compares list elements by key using <paramref name="keyProvider"/>.
        /// </summary>
        public void UseKey(Func<object, object> keyProvider)
        {
            if (keyProvider is null)
            {
                throw new ArgumentNullException(nameof(keyProvider));
            }

            KeyProvider = keyProvider;
        }

        /// <summary>
        /// It will try to find one of the public properties specified by <paramref name="keys"/>, then it returns its value.
        /// </summary>
        /// <returns>Returns the value of the property that corresponds to the specified key. If no property matches the specified key, it returns the <paramref name="instance"/> itself.</returns>
        static object GetKeyValue(object instance, bool caseSensitive, params string[] keys)
        {
            if (instance != null)
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
                if (caseSensitive == false)
                {
                    bindingAttr |= BindingFlags.IgnoreCase;
                }

                foreach (var key in keys)
                {
                    var property = instance.GetType().GetTypeInfo().GetProperty(key, bindingAttr);
                    if (property != null)
                    {
                        return property.GetValue(instance);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Obtains formatted or unformatted <paramref name="elementKey"/>.  See <see cref="FormatElementKey(Func{int, object, string})"/>.
        /// </summary>
        /// <param name="elementIndex"></param>
        /// <param name="elementKey"></param>
        /// <returns></returns>
        internal string GetFormattedElementKey(int elementIndex, object elementKey)
        {
            var formattedKey = ElementKeyFormatter?.Invoke(elementIndex, elementKey);

            if (string.IsNullOrWhiteSpace(formattedKey))
            {
                formattedKey = elementKey.ToString();
            }

            return formattedKey.Left(50);  //This must be enough for a long data type and some prefix. 
        }

        internal string GetFormattedNullElementIdentifier(int elementIndex)
        {
            var elementIdentifier = NullElementIdentifierFormatter?.Invoke(elementIndex);

            if (string.IsNullOrWhiteSpace(elementIdentifier))
            {
                elementIdentifier = string.Format(DefaultNullElementIdentifierTemplate, elementIndex);
            }

            return elementIdentifier.Left(20);
        }

        /// <summary>
        /// To avoid possible confusion of the element key with the element index, the element key can be formatted with any text, e.g. element key with value = 1 can be formatted as "Key=1".
        /// The formatted key is then used as part of the <see cref="Difference.MemberPath"/> property, e.g. "...Addresses[Key=1]".
        /// </summary>
        /// <param name="formatter">First parameter: Element index. Second parameter: Element key. Return value: Formatted key.</param>
        public void FormatElementKey(Func<int, object, string> formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            ElementKeyFormatter = formatter;
        }

        public void FormatNullElementIdentifier(Func<int, string> formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            NullElementIdentifierFormatter = formatter;
        }
    }
}
