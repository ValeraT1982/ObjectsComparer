namespace ObjectsComparer
{
    public interface IValueComparer
    {
        bool Compare(object expected, object actual);

        string ToString(object value);
    }
}
