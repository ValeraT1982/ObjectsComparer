namespace ObjectsComparer
{
    public abstract class AbstractValueComparer: IValueComparer
    {
        public abstract bool Compare(object obj1, object obj2, ComparisonSettings settings);

        public virtual string ToString(object value)
        {
            return value?.ToString();
        }
    }
}
