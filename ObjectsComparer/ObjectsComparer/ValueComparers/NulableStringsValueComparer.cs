namespace ObjectsComparer
{
    /// <summary>
    /// Allows to compare strings considering that null and empty string are equal.
    /// </summary>
    public class NulableStringsValueComparer: IValueComparer
    {
        private static volatile IValueComparer _instance;
        /// <summary>
        /// Static <see cref="NulableStringsValueComparer"/> instance.
        /// </summary>
        public static IValueComparer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new NulableStringsValueComparer();
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Comparers <paramref name="obj1"/> and <paramref name="obj2"/>.
        /// </summary>
        /// <param name="obj1">Object 1.</param>
        /// <param name="obj2">Object 2.</param>
        /// <param name="settings">Instance of <see cref="ComparisonSettings"/> class.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            if (obj1 == null)
            {
                obj1 = string.Empty;
            }

            if (obj2 == null)
            {
                obj2 = string.Empty;
            }

            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Converts values of comparing objects to <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>A string that represents <see cref="value"/>.</returns>
        public string ToString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
