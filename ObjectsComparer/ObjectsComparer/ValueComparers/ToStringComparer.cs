namespace ObjectsComparer
{
    public class ToStringComparer<T> : DynamicValueComparer<T>
    {
        public ToStringComparer() : 
            base((uri1, uri2, settings) => uri1?.ToString() == uri2?.ToString())
        {
        }
    }
}
