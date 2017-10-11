namespace ObjectsComparer
{
    /// <summary>
    /// Implementation of <see cref="IValueComparer"/> which provides simplest implementation of <see cref="ToString(object)"/> method.
    /// </summary>
    public abstract class AbstractValueComparer: IValueComparer
    {
        public abstract bool Compare(object obj1, object obj2, ComparisonSettings settings);

        public virtual string ToString(object value)
        {
            return value?.ToString();
        }
    }
}
