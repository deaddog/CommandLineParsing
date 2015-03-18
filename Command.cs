using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Command
    {
        private Dictionary<string, ArgumentParser> arguments;

        public Command()
        {
            this.arguments = new Dictionary<string, ArgumentParser>();
        }

        public SingleArgumentParser<T> Argument<T>(string name)
        {
            var parser = new SingleArgumentParser<T>(name, getTryParse<T>().Parser);
            arguments.Add(name, parser);
            return parser;
        }
        public ArrayArgumentParser<T> ArrayArgument<T>(string name)
        {
            var parser = new ArrayArgumentParser<T>(name, getTryParse<T>().Parser);
            arguments.Add(name, parser);
            return parser;
        }

        private ParseWrapper<T> getTryParse<T>()
        {
            if (typeof(T) == typeof(int))
                return new ParseWrapper<int>(int.TryParse) as ParseWrapper<T>;

            throw new NotSupportedException("The type " + typeof(T) + " is not supported.");
        }

        private class ParseWrapper<T>
        {
            public readonly TryParse<T> Parser;

            public ParseWrapper(TryParse<T> parser)
            {
                this.Parser = parser;
            }
        }
    }
}
