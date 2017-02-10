namespace ObjectsComparer
{
    public class DefaultValueComparer: IValueComparer
    {
        public bool Compare(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return obj1 == obj2;
            }

            return obj1.Equals(obj2);
        }

        public string ToString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
