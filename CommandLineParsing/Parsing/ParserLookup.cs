using System;
using System.Linq;
using System.Reflection;

namespace CommandLineParsing.Parsing
{
    internal static class ParserLookup
    {
        static ParserLookup()
        {
            _enumParseMethodInfo = typeof(Enum)
                .GetMethods()
                .FirstOrDefault(m => m.Name == nameof(Enum.TryParse) && m.GetParameters().Length == 3 && m.IsGenericMethodDefinition);
        }

        private static MethodInfo _enumParseMethodInfo;

        public static T Parse<T>(ParserSettings parserSettings, string text)
        {
            Message msg;
            T result;

            try
            {
                msg = TryParse<T>(parserSettings, text, out result);
            }
            catch (MissingParserException mpe)
            {
                throw;
            }
            catch (Exception exp)
            {
                throw new ParsingFailedException(typeof(T), exp);
            }

            if (msg.IsError)
                throw new ParsingFailedException(typeof(T), msg.GetMessage());
            else
                return result;
        }
        public static T Parse<T>(ParserSettings parserSettings, string[] text)
        {
            Message msg;
            T result;

            try
            {
                msg = TryParse<T>(parserSettings, text, out result);
            }
            catch (MissingParserException mpe)
            {
                throw;
            }
            catch (Exception exp)
            {
                throw new ParsingFailedException(typeof(T), exp);
            }

            if (msg.IsError)
                throw new ParsingFailedException(typeof(T), msg.GetMessage());
            else
                return result;
        }

        public static Message TryParse<T>(ParserSettings parserSettings, string text, out T result)
        {
            if (typeof(T) == typeof(string))
            {
                result = (T)(object)text;
                return Message.NoError;
            }
            else if (typeof(T).IsEnum)
            {
                var args = new object[] { text, parserSettings.EnumIgnoreCase, null };
                bool parsed = (bool)_enumParseMethodInfo.MakeGenericMethod(typeof(T)).Invoke(null, args);

                result = (T)args[2];
                if (parsed)
                    return Message.NoError;
                else if (parserSettings.UseParserMessage)
                    return $"The value {text} is not defined for the enum {typeof(T).Name}.";
                else
                    return parserSettings.TypeErrorMessage(text);
            }
            else if (TryGetMessageTryParse<T>(out var messageParser))
            {
                Message msg = messageParser(text, out result);
                if (msg.IsError && !parserSettings.UseParserMessage)
                    return parserSettings.TypeErrorMessage(text);
                else
                    return msg;
            }
            else if (TryGetTryParse<T>(out var simpleParser))
            {
                bool parsed = simpleParser(text, out result);
                if (!parsed)
                    return parserSettings.TypeErrorMessage(text);
                else
                    return Message.NoError;
            }
            else
                throw new MissingParserException(typeof(T));
        }
        public static Message TryParse<T>(ParserSettings parserSettings, string[] args, out T result)
        {
            if (typeof(T).IsArray)
            {
                var arrayType = typeof(T).GetElementType();

                var arrMethod = typeof(ParserLookup)
                    .GetMethod(nameof(TryParseArray), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .MakeGenericMethod(arrayType);

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = parserSettings;
                arrMethodArgs[1] = args;

                var msg = arrMethod.Invoke(null, arrMethodArgs);
                result = (T)arrMethodArgs[2];
                return (Message)msg;
            }
            else
                return TryParseSingle(parserSettings, args, out result);
        }

        private static Message TryParseSingle<T>(ParserSettings parserSettings, string[] args, out T result)
        {
            result = default(T);

            if (args.Length == 0)
            {
                if (parserSettings.NoValueMessage.IsError)
                    return parserSettings.NoValueMessage;
                else
                    args = new string[] { string.Empty };
            }
            else if (args.Length > 1)
                return parserSettings.MultipleValuesMessage;

            return TryParse(parserSettings, args[0], out result);
        }
        private static Message TryParseArray<T>(ParserSettings parserSettings, string[] args, out T[] result)
        {
            var arr = new T[args.Length];
            result = null;

            var msg = Message.NoError;
            for (int i = 0; i < args.Length; i++)
                msg += TryParse(parserSettings, args[i], out arr[i]);

            result = arr;
            return msg;
        }

        private static bool TryGetMessageTryParse<T>(out MessageTryParse<T> parser)
        {
            var types = new Type[] { typeof(string), typeof(T).MakeByRefType() };

            var method = typeof(T).GetMethod(nameof(Parsing.MessageTryParse<string>), BindingFlags.Static | BindingFlags.Public, null, types, null);

            if (method?.GetParameters()[1].ParameterType != typeof(T).MakeByRefType() || method?.ReturnType != typeof(Message))
            {
                parser = null;
                return false;
            }

            parser = (MessageTryParse<T>)method.CreateDelegate(typeof(MessageTryParse<>).MakeGenericType(typeof(T)));
            return true;
        }
        private static bool TryGetTryParse<T>(out TryParse<T> parser)
        {
            var types = new Type[] { typeof(string), typeof(T).MakeByRefType() };

            var method = typeof(T).GetMethod(nameof(Parsing.TryParse<string>), BindingFlags.Static | BindingFlags.Public, null, types, null);

            if (method?.GetParameters()[1].ParameterType != typeof(T).MakeByRefType() || method?.ReturnType != typeof(bool))
            {
                parser = null;
                return false;
            }

            parser = (TryParse<T>)method.CreateDelegate(typeof(TryParse<>).MakeGenericType(typeof(T)));
            return true;
        }
    }
}
