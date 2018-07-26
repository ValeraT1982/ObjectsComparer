namespace ObjectsComparer
{
    /// <summary>
    /// Allows to consider provided value and default value of type <see cref="T"/> as equal values.
    /// </summary>
    /// <typeparam name="T">Type of the objects.</typeparam>
    public class DefaultValueValueComparer<T> : IValueComparer
    {
        private readonly T _defaultValue;
        private readonly T _typeDefaultValue;
        private readonly IValueComparer _valueComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueValueComparer{T}" /> class. 
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="valueComparer">Instance of <see cref="IValueComparer"/> which is used when values are not defaults.</param>
        public DefaultValueValueComparer(T defaultValue, IValueComparer valueComparer)
        {
            _defaultValue = defaultValue;
            _valueComparer = valueComparer;
            _typeDefaultValue = default(T);
        }

        /// <summary>
        /// Comparers <paramref name="obj1"/> and <paramref name="obj2"/>.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="settings">Instance of <see cref="ComparisonSettings"/> class.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            var isObj1Default = obj1?.Equals(_defaultValue) != false || obj1.Equals(_typeDefaultValue);
            var isObj2Default = obj2?.Equals(_defaultValue) != false || obj2.Equals(_typeDefaultValue);

            return isObj1Default && isObj2Default || _valueComparer.Compare(obj1, obj2, settings);
        }

        /// <summary>
        /// Converts values of comparing objects to <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>A string that represents <see cref="value"/>.</returns>
        public string ToString(object value)
        {
            return value?.Equals(_typeDefaultValue) != false ? _defaultValue?.ToString() : _valueComparer.ToString(value);
        }
    }
}
