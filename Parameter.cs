using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Defines the base implementation for <see cref="Command"/> parameters.
    /// </summary>
    public abstract class Parameter
    {
#pragma warning disable 1591

        private string name;
        private string[] alternatives;
        private string description;

        private Message required;
        private bool isset;

        private Action callback;
        protected void doCallback()
        {
            if (callback != null)
                callback();
        }

#pragma warning restore

        internal Parameter(string name, string[] alternatives, string description, Message required)
        {
            if (name != null && !RegexLookup.ParameterName.IsMatch(name))
                throw new ArgumentException("Parameter name \"" + name + "\" is illformed.", "name");
            this.name = name;

            foreach (var n in alternatives)
            {
                if (!RegexLookup.ParameterName.IsMatch(n))
                    throw new ArgumentException("Parameter name \"" + n + "\" is illformed.", "alternatives");
            }
            this.alternatives = alternatives;
            this.description = description;
            this.required = required;
            this.isset = false;
        }

        internal abstract Message Handle(string[] values);
        internal abstract bool CanHandle(string value);

        /// <summary>
        /// Manages the action that should be taken when this <see cref="Parameter"/> is identified in an argument set and its values are validated.
        /// </summary>
        public event Action Callback
        {
            add { this.callback += value; }
            remove { this.callback -= value; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Parameter"/> is unnamed (has the <see cref="NoName"/> attribute).
        /// </summary>
        /// <value>
        ///   <c>true</c> if unnamed; otherwise, <c>false</c>.
        /// </value>
        public bool Unnamed
        {
            get { return name == null; }
        }
        /// <summary>
        /// Gets the name of the <see cref="Parameter"/>. This is the first name specified in a <see cref="Name"/> attribute.
        /// </summary>
        /// <value>
        /// The name of <see cref="Parameter"/> if it is named; otherwise, <c>null</c>.
        /// </value>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// Gets the description of this <see cref="Parameter"/>.
        /// </summary>
        /// <value>
        /// The description of this <see cref="Parameter"/>.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="Parameter"/> has been used when executing its containing <see cref="Command"/>.
        /// If <c>false</c> any value contained by the <see cref="Parameter"/> is considered its default value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
        /// </value>
        public bool IsSet
        {
            get { return isset; }
            protected set { isset = value; }
        }

        /// <summary>
        /// Gets all the names associated with this <see cref="Parameter"/>.
        /// </summary>
        /// <param name="includeMain">if set to <c>true</c> the <see cref="Name"/> property is included as the first element in the returned collection.</param>
        /// <returns>A collection of the names (including alternatives) for the <see cref="Parameter"/>.</returns>
        public IEnumerable<string> GetNames(bool includeMain)
        {
            if (includeMain)
                yield return name;
            foreach (var n in alternatives)
                yield return n;
        }
    }
}
