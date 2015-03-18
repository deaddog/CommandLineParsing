using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class ArrayArgumentParser<T> : ArgumentParser<T[], ArrayArgumentParser<T>>
    {
        private TryParse<T> parser;

        internal ArrayArgumentParser(string name, TryParse<T> parser)
            : base(name)
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

        public ArrayArgumentParser<T> ValidateEach(Func<T, Message> validator)
        {
            this.Validate(x =>
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        var msg = validator(x[i]);
                        if (msg != Message.NoError)
                            return msg;
                    }
                    return Message.NoError;
                });

            return this;
        }
        public ArrayArgumentParser<T> ValidateEach(Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            return ValidateEach(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        public ArrayArgumentParser<T> ValidateEach(Func<T, bool> validator, Message errorMessage)
        {
            return ValidateEach(x => validator(x) ? Message.NoError : errorMessage);
        }
    }
}
