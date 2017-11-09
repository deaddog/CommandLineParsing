using System;

namespace CommandLineParsing.Parsing
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
            {
                var arrayType = typeof(T).GetElementType();

                var arrMethod = GetType()
                    .GetMethod(nameof(ParseArray), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .MakeGenericMethod(arrayType);

                var arrMethodArgs = new object[2];
                arrMethodArgs[0] = args;

                var msg = arrMethod.Invoke(this, arrMethodArgs);
                result = (T)arrMethodArgs[1];
                return (Message)msg;
            }
            else
                return ParseSingle(args, out result);
        }

        private Message ParseSingle(string[] args, out T result)
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
        private Message ParseArray<TParse>(string[] args, out TParse[] result)
        {
            var arr = new TParse[args.Length];
            result = null;

            if (ParserLookup.Table.HasMessageTryParse<TParse>())
                for (int i = 0; i < args.Length; i++)
                {
                    Message msg = ParserLookup.Table.MessageTryParse(args[i], out arr[i]);
                    if (msg.IsError)
                        return useParserMessage ? msg : typeErrorMessage(args[i]);
                }
            else if (ParserLookup.Table.HasTryParse<TParse>(enumIgnore))
                for (int i = 0; i < args.Length; i++)
                {
                    if (!ParserLookup.Table.TryParse(enumIgnore, args[i], out arr[i]))
                        return typeErrorMessage(args[i]);
                }
            else
                throw new InvalidOperationException(noParserExceptionMessage);

            result = arr;
            return Message.NoError;
        }
    }
}
