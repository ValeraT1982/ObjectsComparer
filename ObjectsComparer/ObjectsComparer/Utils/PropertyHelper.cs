using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer.Utils
{
    internal class PropertyHelper
    {
        public static MemberInfo GetMemberInfo<T>(Expression<Func<T>> memberLambda)
        {
            MemberExpression exp;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            switch (memberLambda.Body)
            {
                case UnaryExpression body:
                    var unExp = body;
                    if (unExp.Operand is MemberExpression operand)
                    {
                        exp = operand;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }

                    break;
                case MemberExpression _:
                    exp = (MemberExpression)memberLambda.Body;
                    break;
                default:
                    throw new ArgumentException();
            }

            return exp.Member;
        }
    }
}
