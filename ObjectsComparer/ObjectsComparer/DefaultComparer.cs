namespace ObjectsComparer
{
    public class DefaultComparer: IComparer
    {
        public bool Compare(object expected, object actual)
        {
            if (expected == null || actual == null)
            {
                return expected == actual;
            }

            return expected.Equals(actual);
        }

        public string ToString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
