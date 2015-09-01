using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
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
            if (parser == null)
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
