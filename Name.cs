using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies the valid names for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class Name : Attribute
    {
        internal readonly string name;
        internal readonly string[] alternatives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class.
        /// </summary>
        /// <param name="name">The primary name.</param>
        /// <param name="alternatives">An array of alternatives to <paramref name="name"/>.</param>
        public Name(string name, params string[] alternatives)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            this.name = name;

            if (alternatives == null)
                alternatives = new string[0];
            this.alternatives = alternatives;
        }
    }
}
