using System;

namespace ObjectsComparer.Examples.Example2
{
    public class Person
    {
        public Guid CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {MiddleName} {LastName} ({PhoneNumber})";
        }
    }
}
