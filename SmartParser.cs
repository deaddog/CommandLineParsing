namespace CommandLineParsing
{
    internal class SmartParser<T>
    {
        private ParameterTryParse<T> parser;
        private bool enumIgnore;

        public SmartParser(bool enumIgnore)
        {
            this.enumIgnore = enumIgnore;
        }

        public ParameterTryParse<T> Parser
        {
            get { return parser; }
            set { parser = value; }
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
