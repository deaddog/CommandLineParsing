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

        private ParserLookup()
        {
            known = new Dictionary<Type, Delegate>();
            knownIgnore = new Dictionary<Type, Delegate>();

            known.Add(typeof(string), (TryParse<string>)tryParseString);
        }

        private static bool tryParseString(string s, out string result)
        {
            result = s;
            return true;
        }

        public TryParse<T> GetParser<T>(bool enumIgnore)
        {
            Delegate parser;
            if (enumIgnore)
            {
                if (!knownIgnore.TryGetValue(typeof(T), out parser))
                {
                    parser = getParser<T>(enumIgnore);
                    knownIgnore.Add(typeof(T), parser);
                }
            }
            else
            {
                if (!known.TryGetValue(typeof(T), out parser))
                {
                    parser = getParser<T>(enumIgnore);
                    known.Add(typeof(T), parser);
                }
            }
            return parser as TryParse<T>;
        }
        public MessageTryParse<T> GetMessageParser<T>(bool enumIgnore)
        {
            throw new NotImplementedException();
        }

        private static TryParse<T> getParser<T>(bool enumIgnore)
        {
            var type = typeof(T);
            if (type.IsEnum)
                return getParserEnum<T>(enumIgnore);

            var refType = type.MakeByRefType();

            var methods = from m in type.GetMethods()
                          where m.Name == "TryParse" && m.IsStatic && !m.IsGenericMethod && m.ReturnType == typeof(bool)
                          let par = m.GetParameters()
                          where par.Length == 2 && par[0].ParameterType == typeof(string) && par[1].IsOut && par[1].ParameterType == refType
                          select m;

            var method = methods.FirstOrDefault();
            if (method == null)
                throw new NotSupportedException("The type " + typeof(T) + " is not supported. It must provide a static non-generic implementation of the TryParse delegate.");
            else
                return method.CreateDelegate(typeof(TryParse<T>)) as TryParse<T>;
        }
        private static TryParse<T> getParserEnum<T>(bool ignoreCase)
        {
            System.Reflection.MethodInfo method;

            if (!ignoreCase)
                method = (from m in typeof(Enum).GetMethods()
                          where m.Name == "TryParse" && m.IsGenericMethod
                          select m).First();
            else
                method = typeof(ParserLookup).GetMethod("TryParseEnumNoCase", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return method.MakeGenericMethod(typeof(T)).CreateDelegate(typeof(TryParse<T>)) as TryParse<T>;
        }

        private static bool TryParseEnumNoCase<T>(string value, out T result) where T : struct
        {
            return Enum.TryParse<T>(value, true, out result);
        }

        #region Parser conversion

        private static MessageTryParse<T> convert<T>(TryParse<T> parser)
        {
            return new messageDelegateObject<T>(parser).tryParse;
        }
        private static TryParse<T> convert<T>(MessageTryParse<T> parser)
        {
            return new delegateObject<T>(parser).tryParse;
        }

        private class messageDelegateObject<T>
        {
            private TryParse<T> parser;

            public messageDelegateObject(TryParse<T> parser)
            {
                this.parser = parser;
            }

            public Message tryParse(string s, out T value)
            {
                if (!parser(s, out value))
                    return s + " could not be parsed to a value of type " + typeof(T).Name;
                else
                    return Message.NoError;
            }
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
