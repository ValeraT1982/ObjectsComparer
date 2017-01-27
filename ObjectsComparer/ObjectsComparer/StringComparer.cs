namespace ObjectsComparer
{
    public class NulableStringsComparer: IComparer
    {
        public bool Compare(object expected, object actual)
        {
            if (expected == null)
            {
                expected = string.Empty;
            }

            if (actual == null)
            {
                actual = string.Empty;
            }

            return expected.Equals(actual);
        }

        public string ToString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
