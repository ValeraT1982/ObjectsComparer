using System;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Configures the behavior of list elements if list elements are to be compared by key.
    /// </summary>
    public class CompareListElementsByKeyOptions
    {
        /// <summary>
        /// Default element identifier template for element that refers to null value. See <see cref="FormatNullElementIdentifier(Func{int, string})"/> for more info.        
        /// </summary>
        public const string DefaultNullElementIdentifierTemplate = "NullAtIdx={0}";

        /// <summary>
        /// Max. length of the formatted key of the element. See <see cref="FormatElementKey(Func{int, object, string})"/>.
        /// </summary>
        const int FormattedKeyMaxLength = 50;

        /// <summary>
        /// Max. length of the identifier of the element that refers to null value. See <see cref="FormatNullElementIdentifier(Func{int, string})"/>.
        /// </summary>
        const int NullElementIdentifierMaxLength = 20;

        CompareListElementsByKeyOptions()
        {
            Initialize();
        }

        /// <summary>
        /// See <see cref="FormatElementKey(Func{int, object, string})"/>.
        /// </summary>
        Func<FormatListElementKeyArgs, string> ElementKeyFormatter { get; set; }

        /// <summary>
        /// See <see cref="FormatNullElementIdentifier(Func{int, string})"/>.
        /// </summary>
        Func<FormatNullElementIdentifierArgs, string> NullElementIdentifierFormatter { get; set; }

        /// <summary>
        /// See <see cref="ThrowKeyNotFound(bool)"/>.
        /// </summary>
        internal bool ThrowKeyNotFoundEnabled { get; set; } = true;

        /// <summary>
        /// If value = false and element key will not be found, the element will be excluded from comparison and no difference will be added except for possible <see cref="DifferenceTypes.NumberOfElementsMismatch"/> difference.
        /// If value = true and element key will not be found, an exception of type <see cref="ElementKeyNotFoundException"/> will be thrown.
        /// Default value = true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CompareListElementsByKeyOptions ThrowKeyNotFound(bool value)
        {
            ThrowKeyNotFoundEnabled = value;

            return this;
        }

        /// <summary>
        /// See <see cref="UseKey(Func{ListElementKeyProviderArgs, object})"/>.
        /// </summary>
        internal Func<ListElementKeyProviderArgs, object> KeyProviderAction { get; private set; } = null;

        internal static CompareListElementsByKeyOptions Default() => new CompareListElementsByKeyOptions();

        void Initialize()
        {
            UseKey(new string[] { "Id", "Name" }, caseSensitive: false);
        }

        /// <summary>
        /// Key identification. It attempts to find the key using the property specified by the <paramref name="key"/> parameter.
        /// </summary>
        public CompareListElementsByKeyOptions UseKey(string key, bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
            }

            return UseKey(new string[] { key }, caseSensitive);
        }

        /// <summary>
        /// Key identification. It attempts to find the key using one of the public properties specified by the <paramref name="keys"/> parameter, in the specified order.
        /// </summary>
        public CompareListElementsByKeyOptions UseKey(string[] keys, bool caseSensitive = false)
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

            return UseKey(args =>
            {
                return GetKeyValue(args.Element, caseSensitive, keys);
            });
        }

        /// <summary>
        /// Key identification. It attempts to find the key using the <paramref name="keyProvider"/> parameter.
        /// </summary>
        /// <param name="keyProvider">First parameter: The element whose key is required. Return value: The element's key.</param>
        public CompareListElementsByKeyOptions UseKey(Func<ListElementKeyProviderArgs, object> keyProvider)
        {
            if (keyProvider is null)
            {
                throw new ArgumentNullException(nameof(keyProvider));
            }

            KeyProviderAction = keyProvider;

            return this;
        }

        /// <summary>
        /// It will try to find one of the public properties specified by the <paramref name="keys"/> parameter, then it returns its value.
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
        /// Returns optional formatted or unformatted <paramref name="elementKey"/>. See <see cref="FormatElementKey(Func{FormatListElementKeyArgs, string})"/>
        /// </summary>
        internal string GetFormattedElementKey(FormatListElementKeyArgs formatElementKeyArgs)
        {
            if (formatElementKeyArgs is null)
            {
                throw new ArgumentNullException(nameof(formatElementKeyArgs));
            }

            var formattedKey = ElementKeyFormatter?.Invoke(formatElementKeyArgs);

            if (string.IsNullOrWhiteSpace(formattedKey))
            {
                formattedKey = formatElementKeyArgs.ElementKey.ToString();
            }

            return formattedKey.Left(FormattedKeyMaxLength);
        }

        /// <summary>
        /// Returns element identifier for element that referes to null. See <see cref="FormatNullElementIdentifier(Func{int, string})"/>.
        /// </summary>
        /// <param name="args">Element index.</param>
        internal string GetNullElementIdentifier(FormatNullElementIdentifierArgs args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var elementIdentifier = NullElementIdentifierFormatter?.Invoke(args);

            if (string.IsNullOrWhiteSpace(elementIdentifier))
            {
                elementIdentifier = string.Format(DefaultNullElementIdentifierTemplate, args);
            }

            return elementIdentifier.Left(NullElementIdentifierMaxLength);
        }

        /// <summary>
        /// To avoid possible confusion of the element key with the element index, the element key can be formatted with any text.<br/>
        /// For example, element key with value = 1 can be formatted as "Id=1".
        /// The formatted element key is then used as part of the <see cref="Difference.MemberPath"/> property, e.g. "Addresses[Id=1]" instead of "Addresses[1]".<br/>
        /// By default the element key is not formatted.
        /// </summary>
        public CompareListElementsByKeyOptions FormatElementKey(Func<FormatListElementKeyArgs, string> formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            ElementKeyFormatter = formatter;

            return this;
        }

        /// <summary>
        /// Formats the element identifier if it refers to null. Formatted identifier is then used as part of the <see cref="Difference.MemberPath"/> property.<br/>
        /// By default, <see cref="DefaultNullElementIdentifierTemplate"/> template will be used to format the identifier.
        /// </summary>
        /// <param name="formatter">First parameter: Element index. Return value: Formatted identifier.</param>
        public CompareListElementsByKeyOptions FormatNullElementIdentifier(Func<FormatNullElementIdentifierArgs, string> formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            NullElementIdentifierFormatter = formatter;

            return this;
        }
    }
}
