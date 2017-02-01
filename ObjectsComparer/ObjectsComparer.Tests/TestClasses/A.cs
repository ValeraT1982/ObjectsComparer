using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ObjectsComparer.Tests.TestClasses
{
    class A
    {
        public int Field1;

        protected int Field2;

        public int Property1 { get; set; }

        internal double Property2 { get; }

        public B[] ArrayOfB { get; set; }

        public Collection<B> CollectionOfB { get; set; }

        public ICollection<B> ICollectionOf { get; set; }

        public List<B> ListOf { get; set; }

        public CollectionOfB ClassImplementsCollectionOfB { get; set; }

        public int Property3
        {
            set
            {
                if (value > 3)
                {
                    Debug.Print("value > 3");
                }
            }
        }

        public A()
        {
            Property2 = 3.14;
        }

        public A(double property2, int field2)
        {
            Property2 = property2;
            Field2 = field2;
        }
    }
}
