using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    internal class ArrayParameter<T> : Parameter<T[]>
    {
        internal ArrayParameter(string name, string description, Message required)
            : base(name, description, required)
        {
        }
    }
}
