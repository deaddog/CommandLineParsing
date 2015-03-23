using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies the default value for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Default : Attribute
    {
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Default"/> class.
        /// </summary>
        /// <param name="value">The default value for the parameter.
        /// This must be a type that can be cast to the type of the parameter.</param>
        public Default(object value)
        {
            this.value = value;
        }

        internal object Value
        {
            get { return value; }
        }
    }
}
