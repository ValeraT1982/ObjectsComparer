using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    public class PropertyHelper
    {
        public static MemberInfo GetMemberInfo<T>(Expression<Func<T>> memberLambda)
        {
            MemberExpression exp;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            var body = memberLambda.Body as UnaryExpression;
            if (body != null)
            {
                var unExp = body;
                var operand = unExp.Operand as MemberExpression;
                if (operand != null)
                {
                    exp = operand;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (memberLambda.Body is MemberExpression)
            {
                exp = (MemberExpression)memberLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return exp.Member;
        }
    }
}
