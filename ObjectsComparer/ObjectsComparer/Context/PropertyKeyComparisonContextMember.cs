using System;
using System.Reflection;

namespace ObjectsComparer
{
    internal class PropertyKeyComparisonContextMember : IComparisonContextMember
    {
        public PropertyKeyComparisonContextMember(string propertyKey)
        {
            PropertyKey = string.IsNullOrWhiteSpace(propertyKey) ? throw new ArgumentNullException(nameof(propertyKey)) : propertyKey;
        }

        public string PropertyKey { get; }

        public string Name => PropertyKey;

        public MemberInfo Info => null;
    }
}