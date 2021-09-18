using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ObjectsComparer
{
    public class ListConfigurationOptions
    {
        /// <summary>
        /// Whether to compare elements of the lists even if their number differs (<see cref="DifferenceTypes.NumberOfElementsMismatch"/> always will be logged). Default value = false.
        /// </summary>
        public bool CompareUnequalLists { get; set; } = false;

        /// <summary>
        /// If null, the elements will be compared by their index, otherwise by key. Default value = null.
        /// </summary>
        internal Func<object, object> KeyProvider { get; private set;} = null;

        internal static ListConfigurationOptions Default = new ListConfigurationOptions();

        /// <summary>
        /// Compares list elements by index. Default behavior.
        /// </summary>
        public void CompareElementsByIndex()
        {
            KeyProvider = null;
        }

        /// <summary>
        /// Compares list elements by key. It will try to find one of the public properties named "Id" or "Name", in that order. Case sensitive.
        /// </summary>
        public void CompareElementsByKey()
        {
            CompareElementsByKey(caseSensitive: true, "Id", "Name");
        }

        /// <summary>
        /// Compares list elements by key. It will try to find one of the public properties specified by argument <paramref name="keys"/>. In that order.
        /// </summary>
        public void CompareElementsByKey(bool caseSensitive = true, params string[] keys)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (keys.Any() == false)
            {
                throw new ArgumentException("At least one key is required.", nameof(keys));
            }

            CompareElementsByKey(element => 
            {
                return GetKeyValue(element, caseSensitive, keys);
            });
        }

        /// <summary>
        /// Compares list elements by key using <paramref name="keyProvider"/>.
        /// </summary>
        public void CompareElementsByKey(Func<object, object> keyProvider)
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
        /// <returns>Null if no property matches the specified key, or the corresponding property returns null, or <paramref name = "instance" /> is null itself.</returns>
        static object GetKeyValue(object instance, bool caseSensitive, params string[] keys)
        {
            if (instance != null)
            {
                BindingFlags bindingAttr = BindingFlags.Public;
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
    }
}
