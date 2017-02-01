namespace ObjectsComparer
{
    public class ComparisonFailure
    {
        public string MemberPath { get; }

        public string ExpectedValue { get; }

        public string ActualValue { get; }

        public ComparisonFailure(string memberPath, string expectedValue, string actualValue)
        {
            MemberPath = memberPath;
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
        }

        public ComparisonFailure InsertPath(string path)
        {
            return new ComparisonFailure(path + "." + MemberPath, ExpectedValue, ActualValue);
        }
    }
}
