using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies that case should be ignored when parsing input. This only applies to enumeration parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IgnoreCase : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreCase"/> class.
        /// </summary>
        public IgnoreCase()
        {
        }
    }
}
