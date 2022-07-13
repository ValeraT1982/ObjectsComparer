using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectsComparer.Tests.TestClasses
{
    public class Student
    {
        [Description("Člověk")]
        public Person Person { get; set; }
    }
}
