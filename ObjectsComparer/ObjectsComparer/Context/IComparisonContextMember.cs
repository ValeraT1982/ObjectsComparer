using System.Reflection;

namespace ObjectsComparer
{
    public interface IComparisonContextMember
    {
        string Name { get; }

        /// <summary>
        /// Optional. 
        /// </summary>
        MemberInfo Info { get; }
    }
}