using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class FlagParameter : Parameter
    {
        internal FlagParameter(string name, string description, Message required)
            : base(name, description, required)
        {
        }

        internal override Message Handle(Argument argument)
        {
            throw new NotImplementedException();
        }
    }
}
