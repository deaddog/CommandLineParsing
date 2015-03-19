using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Command
    {
        private Dictionary<string, ArgumentParser> arguments;
        private List<ArgumentParser> parsers;

        private List<Func<Message>> validators;
        private Action executor;

        public Command()
        {
            this.arguments = new Dictionary<string, ArgumentParser>();
            this.parsers = new List<ArgumentParser>();

            this.validators = new List<Func<Message>>();
            this.executor = null;
        }

        public Command Validate(Func<Message> validator)
        {
            validators.Add(validator);
            return this;
        }
        public Command Execute(Action executor)
        {
            this.executor += executor;
            return this;
        }

        public virtual Message Validate()
        {
            for (int i = 0; i < validators.Count; i++)
            {
                var msg = validators[i]();
                if (msg.IsError)
                    return msg;
            }

            return Message.NoError;
        }
        public virtual void Execute()
        {
            executor();
        }

        public Message ParseAndExecute(string[] args)
        {
            var unusedParsers = new List<ArgumentParser>(parsers.Where(x => x.IsRequired));
            var argumentStack = CommandLineParsing.Argument.Parse(args);

            while (argumentStack.Count > 0)
            {
                var arg = argumentStack.Pop();
                ArgumentParser parser;
                if (!arguments.TryGetValue(arg.Key, out parser))
                {
                    UnknownArgumentMessage unknown = new UnknownArgumentMessage(arg.Key);
                    foreach (var a in arguments.Keys)
                        unknown.AddAlternative(a, arguments[a].Description);
                    return unknown;
                }

                unusedParsers.Remove(parser);
                var msg = parser.Handle(arg);

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

        public SingleArgumentParser<T> Argument<T>(string name, params string[] alternatives)
        {
            var parser = new SingleArgumentParser<T>(name, parserTable.GetParser<T>());
            parsers.Add(parser);

            if (!RegexLookup.ArgumentName.IsMatch(name))
                throw new ArgumentException("Argument name is illformed.", "name");

            arguments.Add(name, parser);
            foreach (var a in alternatives)
            {
                if (!RegexLookup.ArgumentName.IsMatch(a))
                    throw new ArgumentException("Argument name is illformed.", "alternatives");
                arguments.Add(a, parser);
            }

            return parser;
        }
        public ArrayArgumentParser<T> ArrayArgument<T>(string name, params string[] alternatives)
        {
            var parser = new ArrayArgumentParser<T>(name, parserTable.GetParser<T>());
            parsers.Add(parser);

            if (!RegexLookup.ArgumentName.IsMatch(name))
                throw new ArgumentException("Argument name is illformed.", "name");
            arguments.Add(name, parser);

            foreach (var a in alternatives)
            {
                if (!RegexLookup.ArgumentName.IsMatch(a))
                    throw new ArgumentException("Argument name is illformed.", "alternatives");
                arguments.Add(a, parser);
            }

            return parser;
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
