using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Description : Attribute
    {
        internal readonly string description;

        public Description(string description)
        {
            this.description = description;
        }
    }
}
