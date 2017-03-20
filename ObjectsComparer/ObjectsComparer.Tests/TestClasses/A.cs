using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ObjectsComparer.Tests.TestClasses
{
    class A
    {
        public int Field;

        protected int ProtectedField;

        public readonly string ReadOnlyField;

        public int IntProperty { get; set; }

        public string TestProperty1 { get; set; }

        public string TestProperty2 { get; set; }

        public DateTime DateTimeProperty { get; set; }

        public double ReadOnlyProperty { get; }

        protected bool ProtectedProperty { get; }

        public B ClassB { get; set; }

        public int[] IntArray { get; set; }

        public B[] ArrayOfB { get; set; }

        public Collection<B> CollectionOfB { get; set; }

        // ReSharper disable once InconsistentNaming
        public IEnumerable<B> ICollectionOfB { get; set; }

        public List<B> ListOfB { get; set; }

        public Dictionary<int, B> DictionaryOfB { get; set; }

        public CollectionOfB ClassImplementsCollectionOfB { get; set; }

        public ITestInterface IntefaceProperty { get; set; }

        public TestStruct StructProperty { get; set; }

        public TestEnum EnumProperty { get; set; }

        public IEnumerable NonGenericEnumerable { get; set; }

        public EnumerableImplementation NonGenericEnumerableImplementation { get; set; }

        public int Property3
        {
            set
            {
                if (value > 3)
                {
                    Debug.WriteLine("value > 3");
                }
            }
        }

        public A()
        {
            ReadOnlyProperty = 3.14;
        }

        public A(double readOnlyProperty)
        {
            ReadOnlyProperty = readOnlyProperty;
        }

        public A(int protectedField)
        {
            ProtectedField = protectedField;
        }

        public A(string readOnlyField)
        {
            ReadOnlyField = readOnlyField;
        }

        public A(bool protectedProperty)
        {
            ProtectedProperty = protectedProperty;
        }
    }
}
