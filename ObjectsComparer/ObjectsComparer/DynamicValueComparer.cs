using System;

namespace ObjectsComparer
{
    public class DynamicValueComparer<T>: IValueComparer
    {
        private readonly Func<T, T, bool> _compareFunction;
        private readonly Func<T, string> _toStringFunction;

        public DynamicValueComparer(Func<T, T, bool> compareFunction, Func<T, string> toStringFunction)
        {
            if (compareFunction == null)
            {
                throw new ArgumentNullException(nameof(compareFunction));
            }

            if (toStringFunction == null)
            {
                throw new ArgumentNullException(nameof(toStringFunction));
            }

            _compareFunction = compareFunction;
            _toStringFunction = toStringFunction;
        }

        public bool Compare(object expected, object actual)
        {
            if (expected.GetType() != typeof(T))
            {
                throw new ArgumentException("expected");
            }

            if (actual.GetType() != typeof(T))
            {
                throw new ArgumentException("actual");
            }

            return _compareFunction((T)expected, (T)actual);
        }

        public string ToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value.GetType() != typeof(T))
            {
                throw new ArgumentException("value");
            }

            return _toStringFunction((T)value);
        }
    }
}
