namespace ObjectsComparer
{
    public class NulableStringsValueComparer: IValueComparer
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
