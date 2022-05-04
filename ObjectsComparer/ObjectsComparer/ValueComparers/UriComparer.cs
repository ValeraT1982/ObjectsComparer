using System;

namespace ObjectsComparer
{
    public class UriComparer: DynamicValueComparer<Uri>
    {
        private static volatile UriComparer _instance;
        /// <summary>
        /// Static <see cref="UriComparer"/> instance.
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
                        _instance = new UriComparer();
                    }
                }

                return _instance;
            }
        }

        private static readonly object SyncRoot = new object();

        public UriComparer() : 
            base((uri1, uri2, settings) => uri1.OriginalString == uri2.OriginalString, (uri) => uri.OriginalString)
        {
        }
    }
}
