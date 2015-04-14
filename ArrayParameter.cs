using System;
using System.Linq;

namespace CommandLineParsing
{
    internal class ArrayParameter<T> : Parameter<T[]>
    {
        private TryParse<T> parser;

        internal ArrayParameter(string name, string[] alternatives, string description, Message required, bool enumIgnore)
            : base(name, alternatives, description, required, enumIgnore)
        {
            this.parser = null;
            this.value = new T[0];
        }

        public override T[] Value
        {
            get { return value.ToArray(); }
        }

        internal override Message Handle(Argument argument)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T[] temp = new T[argument.Count];

            for (int i = 0; i < argument.Count; i++)
            {
                if (!parser(argument[i], out temp[i]))
                    return TypeErrorMessage(argument[i]);
            }

            var msg = validator.Validate(temp);
            if (msg.IsError)
                return msg;

            IsSet = true;
            value = temp;
            doCallback();

            return Message.NoError;
        }
        internal override bool CanHandle(string value)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>(enumIgnore);

            T temp;
            return parser(value, out temp);
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}] = {2}{3}",
                Name,
                typeof(T).Name,
                Object.ReferenceEquals(value, null) ? "<null>" : ("{" + string.Join(", ", value) + "}"),
                IsSet ? "" : " (default)");
        }
    }
}
