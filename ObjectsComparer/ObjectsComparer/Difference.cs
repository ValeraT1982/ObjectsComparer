namespace ObjectsComparer
{
    public class Difference
    {
        public string MemberPath { get; }

        public string Value1 { get; }

        public string Value2 { get; }

        public Difference(string memberPath, string value1, string value2)
        {
            MemberPath = memberPath;
            Value1 = value1;
            Value2 = value2;
        }

        public Difference InsertPath(string path)
        {
            var newPath = string.IsNullOrWhiteSpace(MemberPath) || MemberPath.StartsWith("[")
                ? path + MemberPath
                : path + "." + MemberPath;

            return new Difference(
                newPath, 
                Value1, 
                Value2);
        }
    }
}
