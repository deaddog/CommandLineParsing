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

        private Action callback;
        protected void doCallback()
        {
            if (callback != null)
                callback();
        }

        internal Parameter(string name, string description, Message required)
        {
            this.name = name;
            this.description = description;
            this.required = required;
        }

        internal abstract Message Handle(Argument argument);
        internal abstract bool CanHandle(string value);

        public event Action Callback
        {
            add { this.callback += value; }
            remove { this.callback -= value; }
        }

        public string Name
        {
            get { return name ?? "<unnamed>"; }
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
