using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies that a parameter is used unnamed.
    /// Note that there can only be one such parameter and that the parameter type must be an array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NoName : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoName"/> class.
        /// </summary>
        public NoName()
        {
        }
    }
}
