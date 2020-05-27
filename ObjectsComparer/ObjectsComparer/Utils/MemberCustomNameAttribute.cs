using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MemberCustomNameAttribute : Attribute
    {
        public string Name { get; set; }

        public MemberCustomNameAttribute(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
