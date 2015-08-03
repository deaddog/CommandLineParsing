using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies a description for a parameter or command.
    /// The description is used when displaying available parameters/subcommands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Description : Attribute
    {
        internal readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="description">The description associated with the parameter/command.</param>
        public Description(string description)
        {
            this.description = description;
        }
    }
}
