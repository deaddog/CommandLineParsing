using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Message
    {
        private static Message noError = new Message(false);
        public static Message NoError
        {
            get { return noError; }
        }

        private bool isError;
        public bool IsError
        {
            get { return isError; }
        }

        private Message(bool isError)
        {
            this.isError = isError;
        }
        protected Message()
            : this(true)
        {
        }

        public virtual string GetMessage()
        {
            return "";
        }


        public static implicit operator Message(string message)
        {
            return new SimpleMessage(message);
        }

        private class SimpleMessage : Message
        {
            private string message;

            public SimpleMessage(string message)
            {
                this.message = message;
            }

            public override string GetMessage()
            {
                return message;
            }
        }
    }
}
