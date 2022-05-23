﻿using System.Collections;
using System.Collections.Generic;

namespace ObjectsComparer.Tests.TestClasses
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public System.DateTime? Birthdate { get; set; }

        public string PhoneNumber { get; set; }

        public List<Address> ListOfAddress1 { get; set; }

        public List<Address> ListOfAddress2 { get; set; }
    }
}
