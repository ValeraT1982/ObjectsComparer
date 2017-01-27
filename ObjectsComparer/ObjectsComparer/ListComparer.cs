using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public class ListComparer<T> : IComparer
    {
        private IComparer _comparer;

        public ListComparer(IComparer elementComparer = null)
        {
            _comparer = elementComparer ?? new DefaultComparer();
        }

        public bool Compare(object expected, object actual)
        {
            expected = expected ?? new List<T>();
            actual = actual ?? new List<T>();
                        
            if (expected.GetType() != typeof(List<T>))
            {
                throw new ArgumentException("expected");
            }

            if (actual.GetType() != typeof(List<T>))
            {
                throw new ArgumentException("actual");
            }

            var expectedList = (List<T>)expected;
            var actualList = (List<T>)actual;

            if (expectedList.Count != actualList.Count)
            {
                return false;
            }

            return expectedList.All(expectedItem => actualList.Any(actualItem => _comparer.Compare(expectedItem, actualItem)));
        }

        public string ToString(object value)
        {
            value = value ?? new List<T>();

            if (value.GetType() != typeof(List<T>))
            {
                throw new ArgumentException("value");
            }

            var valueList = (List<T>)value;

            return string.Join(",", valueList.Select(v => _comparer.ToString(v)));
        }
    }
}
