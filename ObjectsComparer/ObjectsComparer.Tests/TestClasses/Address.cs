using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectsComparer.Tests.TestClasses
{
    public class Address
    {
        public int Id { get; set; }

        [Description("Aglomerace (město)")]
        public string City { get; set; }

        [Description("Země (stát)")]
        public string Country { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }
    }
}
