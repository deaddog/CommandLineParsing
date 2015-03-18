using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class SingleArgumentParser<T> : ArgumentParser<T, SingleArgumentParser<T>>
    {
        private TryParse<T> parser;

        internal SingleArgumentParser(string name, TryParse<T> parser)
            : base(name)
        {
            this.parser = parser;
        }

        internal override Message Handle(Argument argument)
        {
            T value;

            if (argument.Count == 0)
                return new Message("No value provided for argument \"" + argument + "\".");
            else if (argument.Count > 1)
                return new Message("Only one value can be provided for argument \"" + argument + "\".");
            else if (!parser(argument[0], out value))
                return doTypeValidation(argument[0]);

            return doValidationAndCallback(value);
        }
    }
}
