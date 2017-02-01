using System;

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
                    lock (syncRoot)
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

        private static object syncRoot = new Object();

        private DoNotCompareValueComparer() { }

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
