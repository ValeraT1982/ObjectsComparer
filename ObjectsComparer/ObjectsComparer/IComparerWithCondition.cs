using System;

namespace ObjectsComparer
{
    internal interface IComparerWithCondition: IComparer
    {
        bool IsMatch(Type type);

        bool IsStopComparison(object obj1, object obj2);
    }
}
