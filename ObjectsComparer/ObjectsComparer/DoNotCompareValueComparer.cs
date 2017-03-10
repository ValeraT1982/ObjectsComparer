namespace ObjectsComparer
{
    public class DoNotCompareValueComparer : IValueComparer
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
                            _instance = new DoNotCompareValueComparer();
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly object SyncRoot = new object();

        private DoNotCompareValueComparer() { }

        public bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            return true;
        }

        public string ToString(object value)
        {
            return string.Empty;
        }
    }
}
