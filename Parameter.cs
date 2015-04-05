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
        private string[] alternatives;
        private string description;

        private Message required;

        private Action callback;
        protected void doCallback()
        {
            if (callback != null)
                callback();
        }

        internal Parameter(string name, string[] alternatives, string description, Message required)
        {
            if (name != null && !RegexLookup.ArgumentName.IsMatch(name))
                throw new ArgumentException("Argument name \"" + name + "\" is illformed.", "name");
            this.name = name;

            foreach (var n in alternatives)
            {
                if (!RegexLookup.ArgumentName.IsMatch(n))
                    throw new ArgumentException("Argument name \"" + n + "\" is illformed.", "alternatives");
            }
            this.alternatives = alternatives;
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

        public bool Unnamed
        {
            get { return name == null; }
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

        public IEnumerable<string> GetNames(bool includeMain)
        {
            if (includeMain)
                yield return name;
            foreach (var n in alternatives)
                yield return n;
        }
    }
}
