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
    public class ListElementComparisonByKeyOptions
    {
        /// <summary>
        /// Default element identifier template for element that refers to null value. See <see cref="FormatNullElementIdentifier(Func{int, string})"/> for more info.        
        /// </summary>
        public const string DefaultNullElementIdentifierTemplate = "NullAtIdx={0}";

        /// <summary>
        /// Max. length of the formatted key of the element. See <see cref="FormatElementKey(Func{int, object, string})"/>.
        /// </summary>
        const int FormattedElementKeyMaxLength = 50;

        /// <summary>
        /// Max. length of the identifier of the element that refers to null value. See <see cref="FormatNullElementIdentifier(Func{int, string})"/>.
        /// </summary>
        const int NullElementIdentifierMaxLength = 20;
        
        /// <summary>
        /// Element keys supported by <see cref="DefaultElementKeyProviderAction"/>.
        /// </summary>
        static readonly string[] DefaultElementKeys = new string[] { "Id", "Key", "Name"};

        /// <summary>
        /// 
        /// </summary>
        const bool DefaultElementKeyCaseSensitivity = false;

        ListElementComparisonByKeyOptions()
        {
            DefaultElementKeyProviderAction = args =>
            {
                if (TryGetKeyValueFromElement(args.Element, out var keyValue2))
                {
                    return keyValue2;
                }

                if (TryGetPropertyValue(args.Element, caseSensitive: DefaultElementKeyCaseSensitivity, out var keyValue, DefaultElementKeys))
                {
                    return keyValue;
                }

                return null;

            };

            UseKey(DefaultElementKeyProviderAction);
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
        public ListElementComparisonByKeyOptions ThrowKeyNotFound(bool value)
        {
            ThrowKeyNotFoundEnabled = value;

            return this;
        }

        /// <summary>
        /// See <see cref="UseKey(Func{ListElementKeyProviderArgs, object})"/>.
        /// </summary>
        internal Func<ListElementKeyProviderArgs, object> ElementKeyProviderAction { get; private set; } = null;

        /// <summary>
        /// Default list element key provider. If the element implements <see cref="IEquatable{T}"/>, the provider returns the element itself.
        /// Otherwise, if the element contains one of the properties "Id", "Key", "Name", the provider returns first of them, in that order, even it will be null.
        /// Otherwise the provider returns null.
        /// </summary>
        public Func<ListElementKeyProviderArgs, object> DefaultElementKeyProviderAction { get; private set; } = null;

        internal static ListElementComparisonByKeyOptions Default() => new ListElementComparisonByKeyOptions();

        /// <summary>
        /// Key identification. It attempts to find the key using the property specified by the <paramref name="key"/> parameter.
        /// </summary>
        public ListElementComparisonByKeyOptions UseKey(string key, bool caseSensitive = false)
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
        public ListElementComparisonByKeyOptions UseKey(string[] keys, bool caseSensitive = false)
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
                TryGetPropertyValue(args.Element, caseSensitive, out var propertyValue, keys);
                return propertyValue;
            });
        }

        /// <summary>
        /// Key identification. It attempts to find the key using the <paramref name="keyProvider"/> parameter.
        /// </summary>
        /// <param name="keyProvider">First parameter: The element whose key is required, see <see cref="ListElementKeyProviderArgs"/>. <br/>
        /// Return value: The element's key.</param>
        public ListElementComparisonByKeyOptions UseKey(Func<ListElementKeyProviderArgs, object> keyProvider)
        {
            if (keyProvider is null)
            {
                throw new ArgumentNullException(nameof(keyProvider));
            }

            ElementKeyProviderAction = keyProvider;

            return this;
        }

        /// <summary>
        /// The out parameter <paramref name="value"/> returns first property value of the <paramref name="instance"/> from properties defined by <paramref name="properties"/>, in specified order. Value can be null. 
        /// If no property is found in the object, false is returned by operation.
        /// </summary>
        bool TryGetPropertyValue(object instance, bool caseSensitive, out object value, params string[] properties)
        {
            if (instance != null)
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
                if (caseSensitive == false)
                {
                    bindingAttr |= BindingFlags.IgnoreCase;
                }

                foreach (var key in properties)
                {
                    var property = instance.GetType().GetTypeInfo().GetProperty(key, bindingAttr);
                    if (property != null)
                    {
                        value = property.GetValue(instance);
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// If <paramref name="element"/> is <see cref="IEquatable{T}"/> returns <paramref name="element"/>, otherwise returns null.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        internal static bool TryGetKeyValueFromElement(object element, out object keyValue)
        {
            var elementType = element.GetType();

            if (elementType.InheritsFrom(typeof(System.Collections.Generic.KeyValuePair<,>)))
            {
                keyValue = elementType.GetTypeInfo().GetProperty("Key").GetValue(element);
                return TryGetKeyValueFromElement(keyValue, out keyValue);
            }

            if (elementType.InheritsFrom(typeof(IEquatable<>)))
            {
                keyValue = element;
                return true;
            }

            //if (elementType.InheritsFrom(Nullable.GetUnderlyingType(elementType)))
            //{
            //    keyValue = element;
            //    return true;
            //}

            keyValue = null;
            return false;
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

            return formattedKey.Left(FormattedElementKeyMaxLength);
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
                elementIdentifier = string.Format(DefaultNullElementIdentifierTemplate, args.ElementIndex);
            }

            return elementIdentifier.Left(NullElementIdentifierMaxLength);
        }

        /// <summary>
        /// To avoid possible confusion of the element key with the element index, the element key can be formatted with any text.<br/>
        /// For example, element key with value = 1 can be formatted as "Id=1".
        /// The formatted element key is then used as part of the <see cref="Difference.MemberPath"/> property, e.g. "Addresses[Id=1]" instead of "Addresses[1]".<br/>
        /// By default the element key is not formatted.
        /// </summary>
        public ListElementComparisonByKeyOptions FormatElementKey(Func<FormatListElementKeyArgs, string> formatter)
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
        /// <param name="formatter">
        /// First parameter: Element index.<br/>
        /// Second parameter type: Formatted identifier.
        /// </param>
        public ListElementComparisonByKeyOptions FormatNullElementIdentifier(Func<FormatNullElementIdentifierArgs, string> formatter)
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
