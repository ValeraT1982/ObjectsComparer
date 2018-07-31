using System.Collections;

namespace ObjectsComparer
{
    /// <summary>
    /// Specifies types of the differences between object members.
    /// </summary>
    public enum DifferenceTypes
    {
        /// <summary>
        /// Value of the member in first and second objects are not equal.
        /// </summary>
        ValueMismatch,

        /// <summary>
        /// Type of the member in first and second objects  are not equal.
        /// </summary>
        TypeMismatch,

        /// <summary>
        /// Member does not exist in the first object.
        /// </summary>
        MissedMemberInFirstObject,

        /// <summary>
        /// Member does not exist in the second object.
        /// </summary>
        MissedMemberInSecondObject,

        /// <summary>
        /// First object does not contain element.
        /// </summary>
        MissedElementInFirstObject,

        /// <summary>
        /// Second object does not contain element.
        /// </summary>
        MissedElementInSecondObject,

        /// <summary>
        /// <see cref="IEnumerable"/>s have different number of elements.
        /// </summary>
        NumberOfElementsMismatch
    }
}
