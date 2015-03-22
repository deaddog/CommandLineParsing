using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies that a parameter is required; it must be used when executing the containing command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Required : Attribute
    {
        internal readonly Message message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Required"/> class, using a simple predefined error message.
        /// </summary>
        public Required()
        {
            this.message = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Required"/> class, using a custom error message.
        /// </summary>
        /// <param name="message">The message that should be displayed if the parameter is not used.</param>
        public Required(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.message = message;
        }

        internal static Message defaultMessage(string name)
        {
            return "No value specified for the required parameter \"" + name + "\"";
        }
    }
}
