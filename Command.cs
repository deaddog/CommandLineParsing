using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public abstract partial class Command
    {
        private Dictionary<string, Parameter> parameters;
        private List<Parameter> parsers;

        public Command()
        {
            this.parameters = new Dictionary<string, Parameter>();
            this.parsers = new List<Parameter>();

            this.initializeParameters();
        }

        protected virtual Message Validate()
        {
            return Message.NoError;
        }
        protected virtual void Execute()
        {
        }

        public Message ParseAndExecute(string[] args)
        {
            var unusedParsers = new List<Parameter>(parsers.Where(x => x.IsRequired));
            var argumentStack = CommandLineParsing.Argument.Parse(args);

            while (argumentStack.Count > 0)
            {
                var arg = argumentStack.Pop();
                Parameter parameter;
                if (!parameters.TryGetValue(arg.Key, out parameter))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(arg.Key);
                    var g = parameters.GroupBy(x => x.Value, x => x.Key).Select(x => x.ToArray());
                    foreach (var a in g)
                        unknown.AddAlternative(a, parameters[a[0]].Description);
                    return unknown;
                }

                unusedParsers.Remove(parameter);
                var msg = parameter.Handle(arg);

                if (msg.IsError)
                    return msg;
            }

            if (unusedParsers.Count > 0)
                return unusedParsers[0].RequiredMessage;

            var validMessage = Validate();
            if (validMessage.IsError)
                return validMessage;

            Execute();

            return Message.NoError;
        }

        private static ParserDictionary parserTable = new ParserDictionary();
        #region Implementation of parser retrievel

        private class ParserDictionary
        {
            private Dictionary<Type, Delegate> known;

            public ParserDictionary()
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
                    return getParserEnum<T>();

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
            private static TryParse<T> getParserEnum<T>()
            {
                var methods = from m in typeof(Enum).GetMethods()
                              where m.Name == "TryParse" && m.IsGenericMethod
                              select m;

                return methods.First().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(TryParse<T>)) as TryParse<T>;
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

        #endregion
    }
}
