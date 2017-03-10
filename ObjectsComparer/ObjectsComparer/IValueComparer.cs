namespace ObjectsComparer
{
    public interface IValueComparer
    {
        bool Compare(object obj1, object obj2, ComparisonSettings settings);

        string ToString(object value);
    }
}
