using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Message
    {
        private string message;

        public Message(string message)
        {
            this.message = message;
        }


        public static implicit operator Message(string message)
        {
            return new Message(message);
        }

        public static Message NoError
        {
            get { return null; }
        }
    }
}
