using System;

namespace CommandLineParsing.Parsing
{
    internal static class SmartParser
    {
        public static Message Parse<T>(ParserSettings settings, string[] args, out T result)
        {
            if (typeof(T).IsArray)
            {
                var arrayType = typeof(T).GetElementType();

                var arrMethod = typeof(SmartParser)
                    .GetMethod(nameof(ParseArray), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .MakeGenericMethod(arrayType);

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = settings;
                arrMethodArgs[1] = args;

                var msg = arrMethod.Invoke(null, arrMethodArgs);
                result = (T)arrMethodArgs[2];
                return (Message)msg;
            }
            else
                return ParseSingle(settings, args, out result);
        }

        private static Message ParseSingle<T>(ParserSettings settings, string[] args, out T result)
        {
            result = default(T);

            if (args.Length == 0)
            {
                if (settings.NoValueMessage.IsError)
                    return settings.NoValueMessage;
                else
                    args = new string[] { string.Empty };
            }
            else if (args.Length > 1)
                return settings.MultipleValuesMessage;

            if (ParserLookup.Table.HasMessageTryParse<T>())
            {
                var msg = ParserLookup.Table.MessageTryParse(args[0], out result);
                if (msg.IsError)
                    return settings.UseParserMessage ? msg : settings.TypeErrorMessage(args[0]);
                else
                    return Message.NoError;
            }
            else if (ParserLookup.Table.HasTryParse<T>(settings.EnumIgnoreCase))
                if (!ParserLookup.Table.TryParse(settings.EnumIgnoreCase, args[0], out result))
                    return settings.TypeErrorMessage(args[0]);
                else
                    return Message.NoError;
            else
                throw new InvalidOperationException(settings.NoParserExceptionMessage);
        }
        private static Message ParseArray<T>(ParserSettings settings, string[] args, out T[] result)
        {
            var arr = new T[args.Length];
            result = null;

            if (ParserLookup.Table.HasMessageTryParse<T>())
                for (int i = 0; i < args.Length; i++)
                {
                    Message msg = ParserLookup.Table.MessageTryParse(args[i], out arr[i]);
                    if (msg.IsError)
                        return settings.UseParserMessage ? msg : settings.TypeErrorMessage(args[i]);
                }
            else if (ParserLookup.Table.HasTryParse<T>(settings.EnumIgnoreCase))
                for (int i = 0; i < args.Length; i++)
                {
                    if (!ParserLookup.Table.TryParse(settings.EnumIgnoreCase, args[i], out arr[i]))
                        return settings.TypeErrorMessage(args[i]);
                }
            else
                throw new InvalidOperationException(settings.NoParserExceptionMessage);

            result = arr;
            return Message.NoError;
        }
    }
}
