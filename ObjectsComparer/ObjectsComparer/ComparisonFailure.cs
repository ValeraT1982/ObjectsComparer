namespace ObjectsComparer
{
    public class ComparisonFailure
    {
        public string PropertyName { get; set; }

        public string ExpectedValue { get; set; }

        public string ActualValue { get; set; }
    }
}
