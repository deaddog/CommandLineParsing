using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParsing.Parsing
{
    internal class ParserLookup
    {
        private static ParserLookup parserTable = new ParserLookup();
        public static ParserLookup Table
        {
            get { return parserTable; }
        }

        private Dictionary<Type, Delegate> known;
        private Dictionary<Type, Delegate> knownIgnore;

        private Dictionary<Type, Delegate> knownMessage;

        private ParserLookup()
        {
            known = new Dictionary<Type, Delegate>();
            knownIgnore = new Dictionary<Type, Delegate>();

            knownMessage = new Dictionary<Type, Delegate>();

            known.Add(typeof(string), (TryParse<string>)tryParseString);
            knownMessage.Add(typeof(string), (MessageTryParse<string>)messageTryParseString);
        }

        private static bool tryParseString(string s, out string result)
        {
            result = s;
            return true;
        }
        private static Message messageTryParseString(string s, out string result)
        {
            result = s;
            return Message.NoError;
        }

        static ParserLookup()
        {
            _enumParseMethodInfo = typeof(Enum)
                .GetMethods()
                .FirstOrDefault(m => m.Name == nameof(Enum.TryParse) && m.GetParameters().Length == 3 && m.IsGenericMethodDefinition);
        }

        private static MethodInfo _enumParseMethodInfo;

        public Message TryParse<T>(ParserSettings parserSettings, string text, out T result)
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
            else
                throw new MissingParserException(typeof(T));
        }
        public Message TryParse<T>(ParserSettings parserSettings, string[] args, out T result)
        {
            if (typeof(T).IsArray)
            {
                var arrayType = typeof(T).GetElementType();

                var arrMethod = typeof(ParserLookup)
                    .GetMethod(nameof(TryParseArray), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
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

        private Message TryParseSingle<T>(ParserSettings parserSettings, string[] args, out T result)
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

            if (ParserLookup.Table.HasMessageTryParse<T>())
            {
                var msg = ParserLookup.Table.MessageTryParse(args[0], out result);
                if (msg.IsError)
                    return parserSettings.UseParserMessage ? msg : parserSettings.TypeErrorMessage(args[0]);
                else
                    return Message.NoError;
            }
            else if (ParserLookup.Table.HasTryParse<T>(parserSettings.EnumIgnoreCase))
                if (!ParserLookup.Table.TryParse(parserSettings.EnumIgnoreCase, args[0], out result))
                    return parserSettings.TypeErrorMessage(args[0]);
                else
                    return Message.NoError;
            else
                throw new MissingParserException(typeof(T));
        }
        private Message TryParseArray<T>(ParserSettings parserSettings, string[] args, out T[] result)
        {
            var arr = new T[args.Length];
            result = null;

            if (ParserLookup.Table.HasMessageTryParse<T>())
                for (int i = 0; i < args.Length; i++)
                {
                    Message msg = ParserLookup.Table.MessageTryParse(args[i], out arr[i]);
                    if (msg.IsError)
                        return parserSettings.UseParserMessage ? msg : parserSettings.TypeErrorMessage(args[i]);
                }
            else if (ParserLookup.Table.HasTryParse<T>(parserSettings.EnumIgnoreCase))
                for (int i = 0; i < args.Length; i++)
                {
                    if (!ParserLookup.Table.TryParse(parserSettings.EnumIgnoreCase, args[i], out arr[i]))
                        return parserSettings.TypeErrorMessage(args[i]);
                }
            else
                throw new MissingParserException(typeof(T));

            result = arr;
            return Message.NoError;
        }

        private bool TryGetMessageTryParse<T>(out MessageTryParse<T> parser)
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
        private bool TryGetTryParse<T>(out TryParse<T> parser)
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

        public TryParse<T> GetParser<T>(bool enumIgnore)
        {
            return GetParser(typeof(T), enumIgnore) as TryParse<T>;
        }
        public Delegate GetParser(Type type, bool enumIgnore)
        {
            Delegate parser;
            bool found;

            if (enumIgnore)
                found = knownIgnore.TryGetValue(type, out parser);
            else
                found = known.TryGetValue(type, out parser);

            if (found) return parser;

            parser = getParser(type, enumIgnore);
            if (parser == null && HasMessageTryParse(type))
                parser = convert(type);

            if (enumIgnore)
                knownIgnore.Add(type, parser);
            else
                known.Add(type, parser);

            return parser;
        }
        public MessageTryParse<T> GetMessageParser<T>()
        {
            return GetMessageParser(typeof(T)) as MessageTryParse<T>;
        }
        public Delegate GetMessageParser(Type type)
        {
            Delegate parser;

            if (knownMessage.TryGetValue(type, out parser))
                return parser;

            parser = getMessageParser(type);
            knownMessage.Add(type, parser);

            return parser;
        }

        public bool HasTryParse(Type type, bool enumIgnore)
        {
            return GetParser(type, enumIgnore) != null;
        }
        public bool HasMessageTryParse(Type type)
        {
            return GetMessageParser(type) != null;
        }
        public bool HasTryParse<T>(bool enumIgnore)
        {
            return GetParser<T>(enumIgnore) != null;
        }
        public bool HasMessageTryParse<T>()
        {
            return GetMessageParser<T>() != null;
        }

        public bool TryParse(Type type, bool enumIgnore, string s, out object result)
        {
            object[] args = new object[] { s, getDefault(type) };
            bool r = (bool)GetParser(type, enumIgnore).DynamicInvoke(args);
            result = args[1];
            return r;
        }
        public bool TryParse<T>(bool enumIgnore, string s, out T result)
        {
            return GetParser<T>(enumIgnore)(s, out result);
        }
        public Message MessageTryParse(Type type, string s, out object result)
        {
            object[] args = new object[] { s, getDefault(type) };
            Message r = (Message)GetMessageParser(type).DynamicInvoke(args);
            result = args[1];
            return r;
        }
        public Message MessageTryParse<T>(string s, out T result)
        {
            return GetMessageParser<T>()(s, out result);
        }

        private static object getDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private static Delegate getParser(Type type, bool enumIgnore)
        {
            if (type.IsEnum)
                return getParserEnum(type, enumIgnore);

            var refType = type.MakeByRefType();

            var methods = from m in type.GetMethods()
                          where m.Name == "TryParse" && m.IsStatic && !m.IsGenericMethod && m.ReturnType == typeof(bool)
                          let par = m.GetParameters()
                          where par.Length == 2 && par[0].ParameterType == typeof(string) && par[1].IsOut && par[1].ParameterType == refType
                          select m;

            return methods.FirstOrDefault()?.CreateDelegate(getTryParseType(type));
        }
        private static Delegate getParserEnum(Type type, bool ignoreCase)
        {
            System.Reflection.MethodInfo method;

            if (!ignoreCase)
                method = (from m in typeof(Enum).GetMethods()
                          where m.Name == "TryParse" && m.IsGenericMethod
                          select m).First();
            else
                method = typeof(ParserLookup).GetMethod("TryParseEnumNoCase", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return method.MakeGenericMethod(type).CreateDelegate(getTryParseType(type));
        }
        private static Delegate getMessageParser(Type type)
        {
            var refType = type.MakeByRefType();

            var methods = from m in type.GetMethods()
                          where m.Name == "MessageTryParse" && m.IsStatic && !m.IsGenericMethod && m.ReturnType == typeof(Message)
                          let par = m.GetParameters()
                          where par.Length == 2 && par[0].ParameterType == typeof(string) && par[1].IsOut && par[1].ParameterType == refType
                          select m;

            return methods.FirstOrDefault()?.CreateDelegate(getMessageTryParseType(type));
        }

        private static Type getTryParseType(Type type)
        {
            return typeof(TryParse<>).MakeGenericType(type);
        }
        private static Type getMessageTryParseType(Type type)
        {
            return typeof(MessageTryParse<>).MakeGenericType(type);
        }

        private static bool TryParseEnumNoCase<T>(string value, out T result) where T : struct
        {
            return Enum.TryParse<T>(value, true, out result);
        }

        #region Parser conversion

        private static Delegate convert(Type type)
        {
            var parser = getMessageParser(type);
            var delobjType = typeof(delegateObject<>).MakeGenericType(type);

            object obj = Activator.CreateInstance(delobjType, new object[] { parser });

            return delobjType.GetMethod("tryParse").CreateDelegate(getTryParseType(type), obj);
        }
        private static TryParse<T> convert<T>(MessageTryParse<T> parser)
        {
            return new delegateObject<T>(parser).tryParse;
        }

        private class delegateObject<T>
        {
            private MessageTryParse<T> parser;

            public delegateObject(MessageTryParse<T> parser)
            {
                this.parser = parser;
            }

            public bool tryParse(string s, out T value)
            {
                return !parser(s, out value).IsError;
            }
        }

        #endregion
    }
}
