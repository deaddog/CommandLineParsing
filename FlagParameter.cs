using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a <see cref="Parameter"/> that accepts no values.
    /// </summary>
    public class FlagParameter : Parameter
    {
        private Message hasValueMessage;
        private bool isset;

        internal FlagParameter(string name, string[] alternatives, string description)
            : base(name, alternatives, description, Message.NoError)
        {
            if (this.IsRequired)
                throw new ArgumentException("A FlagParameter cannot be required.", "required");

            isset = false;
            hasValueMessage = name + " is a flag argument, it does not support values.";
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FlagParameter"/> has been used when executing its containing <see cref="Command"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
        /// </value>
        public bool IsSet
        {
            get { return isset; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Message"/> that should be displayed if values are provided for this <see cref="FlagParameter"/>.
        /// </summary>
        /// <value>
        /// The has-value message.
        /// </value>
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

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="FlagParameter"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, isset ? "set" : "not set");
        }
    }
}
