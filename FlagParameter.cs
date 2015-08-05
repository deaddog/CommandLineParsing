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

        internal FlagParameter(string name, string[] alternatives, string description)
            : base(name, alternatives, description, Message.NoError)
        {
            if (this.IsRequired)
                throw new ArgumentException("A FlagParameter cannot be required.", "required");

            hasValueMessage = name + " is a flag argument, it does not support values.";
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

        internal override Message Handle(string[] values)
        {
            if (values.Length > 0)
                return hasValueMessage;

            IsSet = true;
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
            return string.Format("{0}: {1}", Name, IsSet ? "set" : "not set");
        }
    }
}
