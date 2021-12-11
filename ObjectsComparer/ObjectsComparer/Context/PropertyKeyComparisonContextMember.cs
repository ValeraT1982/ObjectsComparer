using System;
using System.Reflection;

namespace ObjectsComparer
{
    internal class PropertyKeyComparisonContextMember : IComparisonContextMember
    {
        public PropertyKeyComparisonContextMember(string propertyKey, MemberInfo info = null)
        {
            PropertyKey = string.IsNullOrWhiteSpace(propertyKey) ? throw new ArgumentNullException(nameof(propertyKey)) : propertyKey;
            Info = info;
        }

        public string PropertyKey { get; }

        public string Name => PropertyKey;

        public MemberInfo Info { get; }
    }
}