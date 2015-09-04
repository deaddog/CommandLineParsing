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

        private Message HandleSingle(string[] args, out T values)
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

            var msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }
        private Message HandleArray(string[] args, out T values)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T[] temp = new T[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (!parser(args[i], out temp[i]))
                    return TypeErrorMessage(args[i]);
            }

            var msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }
    }
}
