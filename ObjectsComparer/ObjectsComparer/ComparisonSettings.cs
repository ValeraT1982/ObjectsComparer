namespace ObjectsComparer
{
    public class ComparisonSettings
    {
        public bool RecursiveComparison { get; set; }

        public bool EmptyAndNullEnumerablesEqual { get; set; }

        public ComparisonSettings()
        {
            RecursiveComparison = true;
            EmptyAndNullEnumerablesEqual = false;
        }
    }
}
