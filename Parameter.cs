using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract class Parameter
    {
        private string name;
        private string description;

        internal Parameter(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        internal abstract Message Handle(Argument argument);

        public string Name
        {
            get { return name; }
        }
        public string Description
        {
            get { return description; }
        }
    }
}
