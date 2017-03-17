using System;

namespace ObjectsComparer
{
    public interface IObjectsDataComparerWithCondition: IObjectsDataComparer
    {
        bool IsMatch(Type type);

        bool IsStopComparison(object obj1, object obj2);
    }
}
