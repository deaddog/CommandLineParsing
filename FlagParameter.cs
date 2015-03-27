using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class FlagParameter : Parameter
    {
        private Message hasValueMessage;
        private bool isset;

        internal FlagParameter(string name, string description, Message required)
            : base(name, description, required)
        {
            if (this.IsRequired)
                throw new ArgumentException("A FlagParameter cannot be required.", "required");

            isset = false;
            hasValueMessage = name + " is a flag argument, it does not support values.";
        }

        public bool IsSet
        {
            get { return isset; }
        }

        public Message HasValueMessage
        {
            get { return hasValueMessage; }
            set { hasValueMessage = value; }
        }

        internal override Message Handle(Argument argument)
        {
            if (argument.Count > 0)
                return hasValueMessage;

            isset = true;
            doCallback();

            return Message.NoError;
        }
        internal override bool CanHandle(string value)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, isset ? "set" : "not set");
        }
    }
}
