using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Required : Attribute
    {
        internal readonly Message message;

        public Required()
        {
            this.message = null;
        }
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
