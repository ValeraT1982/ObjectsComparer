using System;
using System.Collections.Generic;

namespace ObjectsComparer.Examples.Example1
{
    public class Message
    {
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int MessageType { get; set; }

        public int Status { get; set; }

        public List<Error> Errors { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}, Date:{DateCreated}, Type:{MessageType}, Status:{Status}";
        }
    }
}
