using System;
using System.Linq;
using System.Reflection;

namespace CommandLineParsing.Parsing
{
    public class ReflectedParser<T> : IParser<T>
    {
        private readonly ReflectedParserSettings _parserSettings;

        public ReflectedParser(ReflectedParserSettings parserSettings)
        {
            _parserSettings = parserSettings ?? throw new ArgumentNullException(nameof(parserSettings));
        }

        public Message<T> Parse(string[] args)
        {
            var msg = TryParse<T>(_parserSettings, args, out var result);

            if (msg.IsError)
                return new Message<T>(msg.Content);
            else
                return new Message<T>(result);
        }

        private Message TryParse<TParse>(ReflectedParserSettings parserSettings, string text, out TParse result)
        {
            if (typeof(TParse) == typeof(string))
            {
                result = (TParse)(object)text;
                return Message.NoError;
            }
            else if (Nullable.GetUnderlyingType(typeof(TParse)) != null)
            {
                var nullableType = Nullable.GetUnderlyingType(typeof(TParse));

                var nullableMethod = GetType()
                    .GetTypeInfo()
                    .GetDeclaredMethod(nameof(TryParseNullable))
                    .MakeGenericMethod(nullableType);

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = parserSettings;
                arrMethodArgs[1] = text;

                var msg = nullableMethod.Invoke(this, arrMethodArgs);
                result = (TParse)arrMethodArgs[2];
                return (Message)msg;
            }
            else if (typeof(TParse).GetTypeInfo().IsEnum)
            {
                var nullableMethod = GetType()
                    .GetTypeInfo()
                    .GetDeclaredMethod(nameof(TryParseEnum))
                    .MakeGenericMethod(typeof(TParse));

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = parserSettings;
                arrMethodArgs[1] = text;

                var msg = nullableMethod.Invoke(this, arrMethodArgs);
                result = (TParse)arrMethodArgs[2];
                return (Message)msg;
            }
            else if (TryGetMessageTryParse<TParse>(out var messageParser))
            {
                Message msg = messageParser(text, out result);
                if (msg.IsError && !parserSettings.UseParserMessage)
                    return parserSettings.TypeErrorMessage(text);
                else
                    return msg;
            }
            else if (TryGetTryParse<TParse>(out var simpleParser))
            {
                bool parsed = simpleParser(text, out result);
                if (!parsed)
                    return parserSettings.TypeErrorMessage(text);
                else
                    return Message.NoError;
            }
            else
                throw new MissingParserException(typeof(TParse));
        }
        private Message TryParse<TParse>(ReflectedParserSettings parserSettings, string[] args, out TParse result)
        {
            if (typeof(TParse).IsArray)
            {
                var arrayType = typeof(TParse).GetElementType();

                var arrMethod = GetType()
                    .GetTypeInfo()
                    .GetDeclaredMethod(nameof(TryParseArray))
                    .MakeGenericMethod(arrayType);

                var arrMethodArgs = new object[3];
                arrMethodArgs[0] = parserSettings;
                arrMethodArgs[1] = args;

                var msg = arrMethod.Invoke(this, arrMethodArgs);
                result = (TParse)arrMethodArgs[2];
                return (Message)msg;
            }
            else
                return TryParseSingle(parserSettings, args, out result);
        }

        private Message TryParseEnum<TParse>(ReflectedParserSettings parserSettings, string text, out TParse result) where TParse : struct
        {
            if (Enum.TryParse(text, parserSettings.EnumIgnoreCase, out result))
                return Message.NoError;
            else if (parserSettings.UseParserMessage)
                return new Message($"The value {text} is not defined for the enum {typeof(TParse).Name}.");
            else
                return parserSettings.TypeErrorMessage(text);
        }
        private Message TryParseNullable<TParse>(ReflectedParserSettings parserSettings, string text, out TParse? result) where TParse : struct
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result = null;
                return Message.NoError;
            }
            else
            {
                var msg = TryParse<TParse>(parserSettings, text, out var valueResult);
                result = valueResult;

                return msg;
            }
        }
        private Message TryParseSingle<TParse>(ReflectedParserSettings parserSettings, string[] args, out TParse result)
        {
            result = default(TParse);

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
        private Message TryParseArray<TParse>(ReflectedParserSettings parserSettings, string[] args, out TParse[] result)
        {
            var arr = new TParse[args.Length];
            result = null;

            var msg = Message.NoError;
            for (int i = 0; i < args.Length; i++)
                msg += TryParse(parserSettings, args[i], out arr[i]);

            result = arr;
            return msg;
        }

        private bool TryGetMessageTryParse<TParse>(out MessageTryParse<TParse> parser)
        {
            var method = typeof(TParse).GetTypeInfo().GetDeclaredMethods(nameof(Parsing.MessageTryParse<string>)).FirstOrDefault(m =>
            {
                if (!m.IsStatic || !m.IsPublic)
                    return false;

                if (m.ReturnType != typeof(Message))
                    return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2)
                    return false;

                if (parameters[0].ParameterType != typeof(string) || parameters[1].ParameterType != typeof(TParse).MakeByRefType())
                    return false;

                return true;
            });

            if (method is null)
            {
                parser = null;
                return false;
            }

            parser = (MessageTryParse<TParse>)method.CreateDelegate(typeof(MessageTryParse<>).MakeGenericType(typeof(TParse)));
            return true;
        }
        private bool TryGetTryParse<TParse>(out TryParse<TParse> parser)
        {
            var method = typeof(TParse).GetTypeInfo().GetDeclaredMethods(nameof(Parsing.TryParse<string>)).FirstOrDefault(m =>
            {
                if (!m.IsStatic || !m.IsPublic)
                    return false;

                if (m.ReturnType != typeof(bool))
                    return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2)
                    return false;

                if (parameters[0].ParameterType != typeof(string) || parameters[1].ParameterType != typeof(TParse).MakeByRefType())
                    return false;

                return true;
            });

            if (method is null)
            {
                parser = null;
                return false;
            }

            parser = (TryParse<TParse>)method.CreateDelegate(typeof(TryParse<>).MakeGenericType(typeof(TParse)));
            return true;
        }
    }
}
