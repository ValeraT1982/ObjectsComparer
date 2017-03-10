namespace ObjectsComparer
{
    public interface IValueComparer<T>: IValueComparer
    {
        bool Compare(T obj1, T obj2, ComparisonSettings settings);

        string ToString(T value);
    }
}
