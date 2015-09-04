using System;

namespace CommandLineParsing
{
    internal class SmartParser<T>
    {
        private ParameterTryParse<T> parser;
        private bool enumIgnore;

        private Message noValueMessage;
        private Message multipleValuesMessage;
        private Func<string, Message> typeErrorMessage;

        public SmartParser(
            bool enumIgnore,
            Message noValueMessage,
            Message multipleValuesMessage,
            Func<string, Message> typeErrorMessage)
        {
            this.enumIgnore = enumIgnore;
            this.noValueMessage = noValueMessage;
            this.multipleValuesMessage = multipleValuesMessage;
            this.typeErrorMessage = typeErrorMessage;
        }

        public ParameterTryParse<T> Parser
        {
            get { return parser; }
            set { parser = value; }
        }

        public Message NoValueMessage
        {
            get { return noValueMessage; }
            set { noValueMessage = value; }
        }
        public Message MultipleValuesMessage
        {
            get { return multipleValuesMessage; }
            set { multipleValuesMessage = value; }
        }
        public Func<string, Message> TypeErrorMessage
        {
            get { return typeErrorMessage; }
            set { typeErrorMessage = value; }
        }

        public Message Parse(string[] args, out T result)
        {
            if (parser != null)
                return parser(args, out result);
            else if (typeof(T).IsArray)
                return parseArray(args, out result);
            else
                return parseSingle(args, out result);
        }

        private Message parseSingle(string[] args, out T values)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T temp;

            if (args.Length == 0)
                return noValueMessage;
            else if (args.Length > 1)
                return multipleValuesMessage;
            else if (!parser(args[0], out temp))
                return typeErrorMessage(args[0]);

            return Message.NoError;
        }
        private Message parseArray(string[] args, out T values)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T[] temp = new T[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (!parser(args[i], out temp[i]))
                    return TypeErrorMessage(args[i]);
            }

            return Message.NoError;
        }
    }
}
