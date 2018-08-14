namespace ObjectsComparer
{
    /// <summary>
    /// Allows to compare strings considering that null and empty string are equal.
    /// </summary>
    public class NulableStringsValueComparer: AbstractValueComparer<string>
    {
        private static volatile IValueComparer<string> _instance;
        /// <summary>
        /// Static <see cref="NulableStringsValueComparer"/> instance.
        /// </summary>
        public static IValueComparer Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new NulableStringsValueComparer();
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
        public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
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

        public override string ToString(string value)
        {
            return value ?? string.Empty;
        }
    }
}
