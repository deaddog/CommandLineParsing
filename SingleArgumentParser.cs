using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class SingleArgumentParser<T> : ValueArgumentParser<T, SingleArgumentParser<T>>
    {
        private TryParse<T> parser;
        private Func<string, Message> noValueMessage;
        private Func<string, Message> multipleValuesMessage;

        internal SingleArgumentParser(string name, TryParse<T> parser)
            : base(name)
        {
            this.parser = parser;
            this.noValueMessage = x => "No value provided for argument \"" + x + "\".";
            this.multipleValuesMessage = x => "Only one value can be provided for argument \"" + x + "\".";
        }

        internal override Message Handle(Argument argument)
        {
            T value;

            if (argument.Count == 0)
                return noValueMessage(argument.Key);
            else if (argument.Count > 1)
                return multipleValuesMessage(argument.Key);
            else if (!parser(argument[0], out value))
                return doTypeValidation(argument[0]);

            return doValidationAndCallback(value);
        }

        public SingleArgumentParser<T> NoValue(Message errorMessage)
        {
            return NoValue(x => errorMessage);
        }
        public SingleArgumentParser<T> NoValue(Func<string, Message> errorMessage)
        {
            noValueMessage = errorMessage;
            return this;
        }

        public SingleArgumentParser<T> MultipleValues(Message errorMessage)
        {
            return MultipleValues(x => errorMessage);
        }
        public SingleArgumentParser<T> MultipleValues(Func<string, Message> errorMessage)
        {
            multipleValuesMessage = errorMessage;
            return this;
        }
    }
}
