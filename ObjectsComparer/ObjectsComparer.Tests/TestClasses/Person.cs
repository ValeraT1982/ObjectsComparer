using System.Collections;

namespace ObjectsComparer.Tests.TestClasses
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public System.DateTime? Birthdate { get; set; }

        public string PhoneNumber { get; set; }

        public IEnumerable NonGenericLiveAddresses { get; set; }

        public IEnumerable NonGenericStayAddresses { get; set; }
    }
}
