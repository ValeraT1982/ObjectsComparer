//using System.Reflection;

//namespace ObjectsComparer
//{
//    internal class DefaultComparisonContextFactory : IComparisonContextFactory
//    {
//        public IComparisonContext CreateContext(IComparisonContext ancestor = null, string memberName = null)
//        {
//            return new ComparisonContext(CreateMember(memberName, null), ancestor);
//        }

//        public IComparisonContext CreateContext(IComparisonContext ancestor = null, MemberInfo member = null)
//        {
//            return new ComparisonContext(CreateMember(null, member), ancestor);
//        }

//        internal static IComparisonContextMember CreateMember(string memberName, MemberInfo member)
//        {
//            if (member != null)
//            {
//                return new ComparisonContextMember(member);
//            }

//            if (string.IsNullOrWhiteSpace(memberName))
//            {
//                return new ComparisonContextMember();
//            }

//            return new ComparisonContextMember(memberName);
//        }
//    }
//}
