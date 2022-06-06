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
        /// The first object itself.
        /// </summary>
        public object RawValue1 { get; }

        /// <summary>
        /// The second object itself.
        /// </summary>
        public object RawValue2 { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Difference" /> class. 
        /// </summary>
        /// <param name="memberPath">Member Path.</param>
        /// <param name="value1">Value of the first object, converted to string.</param>
        /// <param name="value2">Value of the second object, converted to string.</param>
        /// <param name="rawValue1">The first object itself.</param>
        /// <param name="rawValue2">The second object itself.</param>
        /// <param name="differenceType">Type of the difference.</param>
        public Difference(string memberPath, string value1, string value2, object rawValue1 = null, object rawValue2 = null,
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch)
        {
            MemberPath = memberPath;
            Value1 = value1;
            Value2 = value2;
            DifferenceType = differenceType;
            RawValue1 = rawValue1;
            RawValue2 = rawValue2;
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

            //This instance is probably already included in the difference tree, so I can't create a new one.
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