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
            if (typeof(T) == typeof(byte))
                return new ParseWrapper<byte>(byte.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(sbyte))
                return new ParseWrapper<sbyte>(sbyte.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(short))
                return new ParseWrapper<short>(short.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(ushort))
                return new ParseWrapper<ushort>(ushort.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(int))
                return new ParseWrapper<int>(int.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(uint))
                return new ParseWrapper<uint>(uint.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(long))
                return new ParseWrapper<long>(long.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(ulong))
                return new ParseWrapper<ulong>(ulong.TryParse) as ParseWrapper<T>;

            if (typeof(T) == typeof(float))
                return new ParseWrapper<float>(float.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(double))
                return new ParseWrapper<double>(double.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(decimal))
                return new ParseWrapper<decimal>(decimal.TryParse) as ParseWrapper<T>;

            if (typeof(T) == typeof(bool))
                return new ParseWrapper<bool>(bool.TryParse) as ParseWrapper<T>;

            if (typeof(T) == typeof(DateTime))
                return new ParseWrapper<DateTime>(DateTime.TryParse) as ParseWrapper<T>;

            if (typeof(T) == typeof(char))
                return new ParseWrapper<char>(char.TryParse) as ParseWrapper<T>;
            if (typeof(T) == typeof(string))
                return new ParseWrapper<string>(tryParseString) as ParseWrapper<T>;

            throw new NotSupportedException("The type " + typeof(T) + " is not supported.");
        }

        private bool tryParseString(string s, out string result)
        {
            result = s;
            return true;
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
