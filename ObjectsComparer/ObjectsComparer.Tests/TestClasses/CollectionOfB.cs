using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ObjectsComparer.Tests.TestClasses
{
    internal class CollectionOfB: IProgress<string>, ICollection<B>
    {
        public string Property1 { get; set; }

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        private readonly Collection<B> _collection;
        
        public CollectionOfB()
        {
            _collection = new Collection<B>();
        }

        public void Report(string value) => Debug.WriteLine(value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<B> GetEnumerator() => _collection.GetEnumerator();

        public void Add(B item) => _collection.Add(item);

        public void Clear() => _collection.Clear();

        public bool Contains(B item) => _collection.Contains(item);

        public void CopyTo(B[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public bool Remove(B item) => _collection.Remove(item);
    }
}
