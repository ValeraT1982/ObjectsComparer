using System;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Allows to provide comparison rule as a function.
    /// </summary>
    /// <typeparam name="T">Type of the objects.</typeparam>
    public class DynamicValueComparer<T>: IValueComparer
    {
        private readonly Func<T, T, ComparisonSettings, bool> _compareFunction;
        private readonly Func<T, string> _toStringFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicValueComparer{T}" /> class. 
        /// </summary>
        /// <param name="compareFunction">Function to compare objects of type <see cref="T"/>.</param>
        public DynamicValueComparer(Func<T, T, ComparisonSettings, bool> compareFunction): this(compareFunction, obj => obj?.ToString() ?? string.Empty)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicValueComparer{T}" /> class. 
        /// </summary>
        /// <param name="compareFunction">Function to compare objects of type <see cref="T"/>.</param>
        /// <param name="toStringFunction">Function to convert objects of type <see cref="T"/> to <see cref="string"/>.</param>
        public DynamicValueComparer(Func<T, T, ComparisonSettings, bool> compareFunction, Func<T, string> toStringFunction)
        {
            _compareFunction = compareFunction ?? throw new ArgumentNullException(nameof(compareFunction));
            _toStringFunction = toStringFunction ?? throw new ArgumentNullException(nameof(toStringFunction));
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
            IsArgumentException(obj1, nameof(obj1));
            IsArgumentException(obj2, nameof(obj2));

            return _compareFunction((T)obj1, (T)obj2, settings);
        }

        /// <summary>
        /// Converts values of comparing objects to <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>A string that represents <see cref="value"/>.</returns>
        public string ToString(object value)
        {
            IsArgumentException(value, nameof(value));

            return _toStringFunction((T)value);
        }

        // ReSharper disable once UnusedParameter.Local
        private void IsArgumentException(object obj, string argumentName)
        {
            var t = typeof(T).GetTypeInfo();
            
            if (!(obj is T) && !((t.IsClass || Nullable.GetUnderlyingType(typeof(T)) != null) && obj == null))
            {
                throw new ArgumentException(argumentName);
            }
        }
    }
}
