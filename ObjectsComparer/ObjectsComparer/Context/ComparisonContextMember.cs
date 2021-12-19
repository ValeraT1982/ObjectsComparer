using System;
using System.Reflection;

namespace ObjectsComparer
{
    internal class ComparisonContextMember : IComparisonContextMember
    {
        readonly string _name;

        public ComparisonContextMember()
        {
        }

        public ComparisonContextMember(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            _name = name;
        }

        public ComparisonContextMember(MemberInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public string Name => _name ?? Info?.Name;

        public MemberInfo Info { get; }
    }
}