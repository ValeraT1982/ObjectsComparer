using System;
using System.Collections.Generic;

namespace ObjectsComparer
{
    /// <summary>
    /// Defines methods to compare complex objects.
    /// </summary>
    public interface IComparer : IBaseComparer
    {
        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare(Type type, object obj1, object obj2, out IEnumerable<Difference> differences);

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="differences">List of differences.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare<T>(T obj1, T obj2, out IEnumerable<Difference> differences);

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare(Type type, object obj1, object obj2);

        /// <summary>
        /// Compares objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>True if objects are equal, otherwise false.</returns>
        bool Compare<T>(T obj1, T obj2);

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        IEnumerable<Difference> CalculateDifferences(Type type, object obj1, object obj2);

        /// <summary>
        /// Calculates list of differences between objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <returns>List of differences between objects.</returns>
        IEnumerable<Difference> CalculateDifferences<T>(T obj1, T obj2);
    }
}