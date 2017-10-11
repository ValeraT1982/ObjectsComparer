using System;

namespace ObjectsComparer
{
    /// <summary>
    /// Allows to compare string ignoring case.
    /// </summary>
    public class IgnoreCaseStringsValueComparer : AbstractValueComparer<string>
    {
        private static volatile IValueComparer _instance;
        /// <summary>
        /// Static <see cref="IgnoreCaseStringsValueComparer"/> instance.
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
                            _instance = new IgnoreCaseStringsValueComparer();
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
        public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
        {
            return string.Compare(obj1, obj2, StringComparison.CurrentCultureIgnoreCase) == 0;
        }
    }
}
