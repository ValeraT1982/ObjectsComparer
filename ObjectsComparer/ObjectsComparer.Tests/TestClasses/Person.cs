using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectsComparer.Tests.TestClasses
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public System.DateTime? Birthdate { get; set; }

        public string PhoneNumber { get; set; }

        [Description("Kolekce adres")]
        public List<Address> ListOfAddress1 { get; set; }

        public List<Address> ListOfAddress2 { get; set; }
    }
}
