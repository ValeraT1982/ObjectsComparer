namespace ObjectsComparer
{
    /// <summary>
    /// Implementation of <see cref="IValueComparer{T}"/> which provides simplest implementation of <see cref="ToString(T)"/> method and 
    /// <see cref="Compare(object, object, ComparisonSettings)"/> method to call <see cref="Compare(T, T, ComparisonSettings)"/>.
    /// </summary>
    /// <typeparam name="T">Type of the objects.</typeparam>
    public abstract class AbstractValueComparer<T>: AbstractValueComparer, IValueComparer<T>
    {
        /// <summary>
        /// Comparers <paramref name="obj1"/> and <paramref name="obj2"/>.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="settings">Instance of <see cref="ComparisonSettings"/> class.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public abstract bool Compare(T obj1, T obj2, ComparisonSettings settings);

        /// <summary>
        /// Converts values of comparing objects to <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>A string that represents <see cref="value"/>.</returns>
        public virtual string ToString(T value)
        {
            return base.ToString(value);
        }

        /// <summary>
        /// Comparers <paramref name="obj1"/> and <paramref name="obj2"/>.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="settings">Instance of <see cref="ComparisonSettings"/> class.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            return Compare((T) obj1, (T) obj2, settings);
        }
    }
}
