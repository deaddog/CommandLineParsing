using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies the valid names for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class Name : Attribute
    {
        internal readonly string[] names;

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class.
        /// </summary>
        /// <param name="name">The primary name.</param>
        /// <param name="alternatives">An array of alternatives to <paramref name="name"/>.</param>
        public Name(string name, params string[] alternatives)
        {
            this.names = new string[1 + alternatives.Length];
            this.names[0] = name;
            alternatives.CopyTo(this.names, 1);
        }
    }
}
