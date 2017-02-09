using System.Collections;

namespace ObjectsComparer.Tests.TestClasses
{
    class EnumerableImplementation: IEnumerable
    {
        public string Property1 { get; set; }

        private readonly IEnumerable _enumerable;

        public EnumerableImplementation(IEnumerable enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }
    }
}
