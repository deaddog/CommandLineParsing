using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class ArrayArgumentParser<T> : ArgumentParser<T[]>
    {
        private TryParse<T> parser;

        public ArrayArgumentParser(string name, TryParse<T> parser)
            : base(name, null)
        {
            this.parser = parser;
        }

        internal override Message Handle(Argument argument)
        {
            T[] values = new T[argument.Count];

            for (int i = 0; i < argument.Count; i++)
            {
                if (!parser(argument[i], out values[i]))
                    return doTypeValidation(argument[i]);
            }

            return doValidationAndCallback(values);
        }
    }
}
