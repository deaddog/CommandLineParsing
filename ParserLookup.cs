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

        private ParserLookup()
        {
            known = new Dictionary<Type, Delegate>();
        }

        public TryParse<T> GetParser<T>()
        {
            Delegate parser;
            if (!known.TryGetValue(typeof(T), out parser))
            {
                parser = getParser<T>();
                known.Add(typeof(T), parser);
            }
            return parser as TryParse<T>;
        }

        private static TryParse<T> getParser<T>()
        {
            if (typeof(T) == typeof(string))
                return wrapParser<string, T>(tryParseString);

            var type = typeof(T);
            if (type.IsEnum)
                return getParserEnum<T>(false);

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

        private static bool tryParseString(string s, out string result)
        {
            result = s;
            return true;
        }

        private static TryParse<TTo> wrapParser<TFrom, TTo>(TryParse<TFrom> parser)
        {
            return parser as TryParse<TTo>;
        }
    }
}
