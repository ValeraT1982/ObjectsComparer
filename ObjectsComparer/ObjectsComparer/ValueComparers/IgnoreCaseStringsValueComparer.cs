using System;

namespace ObjectsComparer
{
    public class IgnoreCaseStringsValueComparer : AbstractValueComparer<string>
    {
        private static volatile IValueComparer _instance;
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

        public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
        {
            return string.Compare(obj1, obj2, StringComparison.CurrentCultureIgnoreCase) == 0;
        }
    }
}
