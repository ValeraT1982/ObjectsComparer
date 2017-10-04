namespace ObjectsComparer
{
    public abstract class AbstractValueComparer<T>: AbstractValueComparer, IValueComparer<T>
    {
        public abstract bool Compare(T obj1, T obj2, ComparisonSettings settings);

        public virtual string ToString(T value)
        {
            return base.ToString(value);
        }

        public override bool Compare(object obj1, object obj2, ComparisonSettings settings)
        {
            return Compare((T) obj1, (T) obj2, settings);
        }
    }
}
