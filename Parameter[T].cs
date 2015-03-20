using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Parameter<T> : Parameter
    {
        internal Parameter(string name, string description)
            : base(name, description)
        {
        }
    }
}
