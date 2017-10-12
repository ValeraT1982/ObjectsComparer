using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Defines methods to compare complex objects of particular type.
    /// </summary>
    public interface IComparer<in T>: IBaseComparer
    {
        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare(T obj1, T obj2, out IEnumerable<Difference> differences);

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare(T obj1, T obj2);

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        IEnumerable<Difference> CalculateDifferences(T obj1, T obj2);
    }
}