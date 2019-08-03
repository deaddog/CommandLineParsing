using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandLineParsing.Tests")]
namespace CommandLineParsing.Parsing
{
    internal static class ParserLookup
    {
        static ParserLookup()
        {
            _enumParseMethodInfo = typeof(Enum)
                .GetTypeInfo()
                .GetDeclaredMethods(nameof(Enum.TryParse))
                .FirstOrDefault(m => m.GetParameters().Length == 3 && m.IsGenericMethodDefinition);
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
            catch (MissingParserException)
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
            catch (MissingParserException)
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
            else if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                var nullableType = Nullable.GetUnderlyingType(typeof(T));

                var nullableMethod = typeof(ParserLookup)
                    .GetTypeInfo()
                    .GetDeclaredMethod(nameof(TryParseNullable))
                    .MakeGenericMethod(nullableType);

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = parserSettings;
                arrMethodArgs[1] = text;

                var msg = nullableMethod.Invoke(null, arrMethodArgs);
                result = (T)arrMethodArgs[2];
                return (Message)msg;
            }
            else if (typeof(T).GetTypeInfo().IsEnum)
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
                    .GetTypeInfo()
                    .GetDeclaredMethod(nameof(TryParseArray))
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

        private static Message TryParseNullable<T>(ParserSettings parserSettings, string text, out T? result) where T : struct
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result = null;
                return Message.NoError;
            }
            else
            {
                var msg = TryParse<T>(parserSettings, text, out var valueResult);
                result = valueResult;

                return msg;
            }
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
            var method = typeof(T).GetTypeInfo().GetDeclaredMethods(nameof(Parsing.MessageTryParse<string>)).FirstOrDefault(m =>
            {
                if (!m.IsStatic || !m.IsPublic)
                    return false;

                if (m.ReturnType != typeof(Message))
                    return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2)
                    return false;

                if (parameters[0].ParameterType != typeof(string) || parameters[1].ParameterType != typeof(T).MakeByRefType())
                    return false;

                return true;
            });

            if (method is null)
            {
                parser = null;
                return false;
            }

            parser = (MessageTryParse<T>)method.CreateDelegate(typeof(MessageTryParse<>).MakeGenericType(typeof(T)));
            return true;
        }
        private static bool TryGetTryParse<T>(out TryParse<T> parser)
        {
            var method = typeof(T).GetTypeInfo().GetDeclaredMethods(nameof(Parsing.TryParse<string>)).FirstOrDefault(m =>
            {
                if (!m.IsStatic || !m.IsPublic)
                    return false;

                if (m.ReturnType != typeof(bool))
                    return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2)
                    return false;

                if (parameters[0].ParameterType != typeof(string) || parameters[1].ParameterType != typeof(T).MakeByRefType())
                    return false;

                return true;
            });

            if (method is null)
            {
                parser = null;
                return false;
            }

            parser = (TryParse<T>)method.CreateDelegate(typeof(TryParse<>).MakeGenericType(typeof(T)));
            return true;
        }
    }
}
