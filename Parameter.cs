using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract class Parameter
    {
        private string name;
        private string description;

        private Message required;

        internal Parameter(string name, string description, Message required)
        {
            this.name = name;
            this.description = description;
            this.required = required;
        }

        internal abstract Message Handle(Argument argument);

        public string Name
        {
            get { return name; }
        }
        public string Description
        {
            get { return description; }
        }

        internal bool IsRequired
        {
            get { return required.IsError; }
        }
        internal Message RequiredMessage
        {
            get { return required; }
        }
    }
}
