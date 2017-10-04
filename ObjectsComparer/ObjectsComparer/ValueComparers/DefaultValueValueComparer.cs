namespace ObjectsComparer
{
    public class DefaultValueValueComparer<T> : IValueComparer
    {
        private readonly T _defaultValue;
        private readonly T _typeDefaultValue;
        private readonly IValueComparer _valueComparer;

        public DefaultValueValueComparer(T defaultValue, IValueComparer valueComparer)
        {
            _defaultValue = defaultValue;
            _valueComparer = valueComparer;
            _typeDefaultValue = default(T);
        }

        public bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            var isObj1Default = obj1?.Equals(_defaultValue) != false || obj1.Equals(_typeDefaultValue);
            var isObj2Default = obj2?.Equals(_defaultValue) != false || obj2.Equals(_typeDefaultValue);

            return (isObj1Default && isObj2Default) || _valueComparer.Compare(obj1, obj2, settings);
        }

        public string ToString(object value)
        {
            return value?.ToString();
        }
    }
}
