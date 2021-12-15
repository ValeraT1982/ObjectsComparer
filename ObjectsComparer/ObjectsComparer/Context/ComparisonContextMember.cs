using System.Reflection;

namespace ObjectsComparer
{
    internal class ComparisonContextMember : IComparisonContextMember
    {
        public ComparisonContextMember()
        {
        }

        public ComparisonContextMember(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public ComparisonContextMember(MemberInfo info)
        {
            Info = info ?? throw new System.ArgumentNullException(nameof(info));
        }

        public string Name { get; }

        public MemberInfo Info { get; }
    }
}