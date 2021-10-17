namespace ObjectsComparer
{
    /// <summary>
    /// Represents difference in one member between objects.
    /// </summary>
    public class Difference
    {
        /// <summary>
        /// Path to the member.
        /// </summary>
        public string MemberPath { get; private set; }

        /// <summary>
        /// Value in the first object, converted to string.
        /// </summary>
        public string Value1 { get; }

        /// <summary>
        /// Value in the second object, converted to string.
        /// </summary>
        public string Value2 { get; }

        /// <summary>
        /// Type of the difference.
        /// </summary>
        public DifferenceTypes DifferenceType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Difference" /> class. 
        /// </summary>
        /// <param name="memberPath">Member Path.</param>
        /// <param name="value1">Value of the first object, converted to string.</param>
        /// <param name="value2">Value of the second object, converted to string.</param>
        /// <param name="differenceType">Type of the difference.</param>
        public Difference(string memberPath, string value1, string value2,
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch)
        {
            MemberPath = memberPath;
            Value1 = value1;
            Value2 = value2;
            DifferenceType = differenceType;
        }

        /// <summary>
        /// Combines difference with path of the root element.
        /// </summary>
        /// <param name="path">Root element path.</param>
        /// <returns>Difference with combined <see cref="MemberPath"/>.</returns>
        public Difference InsertPath(string path)
        {
            var newPath = string.IsNullOrWhiteSpace(MemberPath) || MemberPath.StartsWith("[")
                ? path + MemberPath
                : path + "." + MemberPath;

            //return new Difference(
            //    newPath,
            //    Value1,
            //    Value2,
            //    DifferenceType);

            MemberPath = newPath;

            return this;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Difference: DifferenceType={DifferenceType}, MemberPath='{MemberPath}', Value1='{Value1}', Value2='{Value2}'.";
        }
    }
}