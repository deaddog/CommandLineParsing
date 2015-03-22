using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies a description for a parameter.
    /// The description is use when displaying available parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Description : Attribute
    {
        internal readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="description">The description associated with the parameter.</param>
        public Description(string description)
        {
            this.description = description;
        }
    }
}
