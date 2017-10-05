namespace ObjectsComparer
{
    public class NulableStringsValueComparer: IValueComparer
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
                            _instance = new NulableStringsValueComparer();
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly object SyncRoot = new object();

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

        public string ToString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
