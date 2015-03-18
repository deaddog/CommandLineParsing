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
            var parser = getParser<T>(name);
            arguments.Add(name, parser);
            return parser;
        }
        private SingleArgumentParser<T> getParser<T>(string name)
        {
            if (typeof(T) == typeof(int))
                return new SingleArgumentParser<int>(name, int.TryParse) as SingleArgumentParser<T>;

            throw new NotSupportedException("The type " + typeof(T) + " is not supported.");
        }

        public ArrayArgumentParser<T> ArrayArgument<T>(string name)
        {
            var parser = getArrayParser<T>(name);
            arguments.Add(name, parser);
            return parser;
        }
        public ArrayArgumentParser<T> getArrayParser<T>(string name)
        {
            if (typeof(T) == typeof(int))
                return new ArrayArgumentParser<int>(name, int.TryParse) as ArrayArgumentParser<T>;

            throw new NotSupportedException("The type " + typeof(T) + " is not supported.");
        }
    }
}
