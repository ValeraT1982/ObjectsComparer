namespace ObjectsComparer
{
    public class DoNotCompareComparer : IComparer
    {
        private static volatile IComparer _instance;
        public static IComparer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new DoNotCompareComparer();
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly object SyncRoot = new object();

        private DoNotCompareComparer() { }

        public bool Compare(object expected, object actual)
        {
            return true;
        }

        public string ToString(object value)
        {
            return string.Empty;
        }
    }
}
