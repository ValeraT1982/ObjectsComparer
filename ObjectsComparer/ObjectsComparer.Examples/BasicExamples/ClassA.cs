namespace ObjectsComparer.Examples.BasicExamples
{
    public class ClassA
    {
        public string StringProperty { get; set; }

        public int IntProperty { get; set; }

        public SubClassA SubClass { get; set; }
    }

    public class SubClassA
    {
        public bool BoolProperty { get; set; }
    }
}
