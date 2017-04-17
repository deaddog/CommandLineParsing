using System;

namespace CommandLineParsing
{
    internal class SmartParser<T>
    {
        private ParameterTryParse<T> parser;
        private bool enumIgnore;
        private string noParserExceptionMessage;

        private Message noValueMessage;
        private Message multipleValuesMessage;
        private Func<string, Message> typeErrorMessage;
        private bool useParserMessage;

        public SmartParser()
        {
        }

        public ParameterTryParse<T> Parser
        {
            get { return parser; }
            set { parser = value; }
        }

        public bool EnumIgnoreCase
        {
            get { return enumIgnore; }
            set { enumIgnore = value; }
        }
        public string NoParserExceptionMessage
        {
            get { return noParserExceptionMessage; }
            set { noParserExceptionMessage = value; }
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
        public bool UseParserMessage
        {
            get { return useParserMessage; }
            set { useParserMessage = value; }
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

        private Message parseSingle(string[] args, out T result)
        {
            result = default(T);

            if (args.Length == 0)
            {
                if (noValueMessage.IsError)
                    return noValueMessage;
                else
                    args = new string[] { string.Empty };
            }
            else if (args.Length > 1)
                return multipleValuesMessage;

            if (ParserLookup.Table.HasMessageTryParse<T>())
            {
                var msg = ParserLookup.Table.MessageTryParse(args[0], out result);
                if (msg.IsError)
                    return useParserMessage ? msg : typeErrorMessage(args[0]);
                else
                    return Message.NoError;
            }
            else if (ParserLookup.Table.HasTryParse<T>(enumIgnore))
                if (!ParserLookup.Table.TryParse(enumIgnore, args[0], out result))
                    return typeErrorMessage(args[0]);
                else
                    return Message.NoError;
            else
                throw new InvalidOperationException(noParserExceptionMessage);
        }
        private Message parseArray(string[] args, out T result)
        {
            Type t = typeof(T).GetElementType();
            Array arr = (Array)Activator.CreateInstance(typeof(T), new object[] { args.Length });

            result = default(T);

            if (ParserLookup.Table.HasMessageTryParse(t))
                for (int i = 0; i < args.Length; i++)
                {
                    object o;
                    Message msg = ParserLookup.Table.MessageTryParse(t, args[i], out o);
                    if (msg.IsError)
                        return useParserMessage ? msg : typeErrorMessage(args[i]);
                    arr.SetValue(o, i);
                }
            else if (ParserLookup.Table.HasTryParse(t, enumIgnore))
                for (int i = 0; i < args.Length; i++)
                {
                    object o;
                    if (!ParserLookup.Table.TryParse(t, enumIgnore, args[i], out o))
                        return typeErrorMessage(args[i]);
                    arr.SetValue(o, i);
                }
            else
                throw new InvalidOperationException(noParserExceptionMessage);

            result = (T)(object)arr;
            return Message.NoError;
        }
    }
}
