using System;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    /// <summary>
    /// Configures list element behavior if <see cref="ListElementSearchMode.Key"/> mode is used.
    /// </summary>
    public class CompareElementsByKeyOptions
    {
        /// <summary>
        /// Default key prefix for an integer key. It will be used for formatting <see cref="Difference.MemberPath"/> property, for example: "Addresses[KEY=123]".
        /// See <see cref="KeyPrefix"/> for more info.
        /// </summary>
        /// <example>Addresses</example>
        public const string DefaultIntKeyPrefix = "KEY=";

        /// <summary>
        /// Default element identifier for element that refers to null. It will be used for formatting <see cref="Difference.MemberPath"/> property, for example: "Addresses[NULLREF]". 
        /// See <see cref="NullElementIdentifier"/> for more info.        
        /// </summary>
        public const string DefaultNullElementIdentifier = "NULLREF";

        CompareElementsByKeyOptions()
        {
            Initialize();
        }

        internal static CompareElementsByKeyOptions Default() => new CompareElementsByKeyOptions();

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
        /// To avoid confusion with the index, an optional key element prefix can be used. If value = null, which is the default value, <see cref="DefaultIntKeyPrefix"/> will be used for integer key type and no prefix will be used for other types.
        /// If you don't want to use prefix at all, set this property to <see cref="string.Empty"/>.
        /// </summary>
        public string KeyPrefix { get; set; } = null;

        /// <summary>
        /// If the list element refers to a null value, this symbol will eventually be used as list element identifier in the <see cref="Difference.MemberPath"/> property.
        /// Default value = <see cref="DefaultNullElementIdentifier"/>.
        /// If you don't want to use it at all, set this property to <see cref="string.Empty"/>.
        /// </summary>
        public string NullElementIdentifier { get; set; } = DefaultNullElementIdentifier;

        void Initialize()
        {
            UseKey(new string[] { "Id", "Name" }, caseSensitive: false);
        }

        /// <summary>
        /// Compares list elements by key. It will try to find a property specified by argument <paramref name="key"/>.
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
        /// Compares list elements by key. It will try to find one of the public properties specified by argument <paramref name="keys"/>, in that order.
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
        /// See <see cref="KeyPrefix"/>.
        /// </summary>
        internal static string ResolveKeyPrefix(object elementKey, CompareElementsByKeyOptions options)
        {
            if (elementKey is null)
            {
                throw new ArgumentNullException(nameof(elementKey));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.KeyPrefix == string.Empty)
            {
                return string.Empty;
            }

            if (options.KeyPrefix == null)
            {
                if (elementKey is byte || elementKey is short || elementKey is int || elementKey is long)
                {
                    return DefaultIntKeyPrefix;
                }
            }

            return options.KeyPrefix.Left(10);
        }

        internal static string ResolveNullElementIdentifier(CompareElementsByKeyOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.NullElementIdentifier == null) 
            {
                return string.Empty;
            }

            return options.NullElementIdentifier.Left(10);
        }
    }
}
