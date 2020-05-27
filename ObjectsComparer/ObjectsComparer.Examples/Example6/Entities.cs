using ObjectsComparer.CustomComparers;
using ObjectsComparer.Utils;
using System.Collections.Generic;

namespace ObjectsComparer.Examples.Example7
{
    [GroupName("Reservation")]
    public class Element
    {
        public string Name { get; set; }

        [MemberCustomName("Rooms")]
        public IList<ElementItem> Items { get; set; }
    }

    [GroupName("Room")]
    public class ElementItem : IComparableEnumerableItem
    {
        public int ElementId { get; set; }
        public string Description { get; set; }
        public ElementPrice Price { get; set; }

        public string Key => ElementId.ToString();
    }

    [GroupName("RoomPrice")]
    public class ElementPrice
    {
        public decimal Value { get; set; }
    }
}
