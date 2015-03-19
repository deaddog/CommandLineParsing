using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract class ArgumentParser
    {
        private string name;
        internal string Name
        {
            get { return name; }
        }

        internal abstract string Description
        {
            get;
        }

        internal ArgumentParser(string name)
        {
            this.name = name;
        }

        internal abstract bool IsRequired
        {
            get;
        }
        internal abstract Message RequiredMessage
        {
            get;
        }
        internal abstract Message Handle(Argument argument);
    }
}
