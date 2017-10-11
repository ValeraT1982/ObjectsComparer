namespace ObjectsComparer
{
    /// <summary>
    /// Defines a generalized comparison method to compare 2 objects.
    /// </summary>
    public interface IValueComparer
    {
        /// <summary>
        /// Comparers <paramref name="obj1"/> and <paramref name="obj2"/>.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="settings">Instance of <see cref="ComparisonSettings"/> class.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        bool Compare(object obj1, object obj2, ComparisonSettings settings);

        /// <summary>
        /// Converts values of comparing objects to <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>A string that represents <see cref="value"/>.</returns>
        string ToString(object value);
    }
}
