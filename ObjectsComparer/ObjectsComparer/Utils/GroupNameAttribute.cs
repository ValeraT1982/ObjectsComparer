using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer.Utils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupNameAttribute : Attribute
    {
        public string Name { get; set; }

        public GroupNameAttribute(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
