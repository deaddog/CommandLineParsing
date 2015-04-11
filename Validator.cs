using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Validator<T>
    {
        private List<Func<T, Message>> validators;

        public Validator()
        {
            this.validators = new List<Func<T, Message>>();
        }
    }
}
