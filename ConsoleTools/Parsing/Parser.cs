using System;

namespace ConsoleTools.Parsing
{
    public class Parser<T> : IParser<T>
    {
        private readonly Func<string, Message<T>> _parser;

        public Parser(Func<string, Message<T>> parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public Message<T> Parse(string arg)
        {
            return _parser(arg);
        }
    }
}
