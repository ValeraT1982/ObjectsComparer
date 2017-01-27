namespace ObjectsComparer
{
    public interface IComparer
    {
        bool Compare(object expected, object actual);

        string ToString(object value);
    }
}
