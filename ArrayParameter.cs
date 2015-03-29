using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    internal class ArrayParameter<T> : Parameter<T[]>
    {
        private TryParse<T> parser;

        internal ArrayParameter(string name, string description, Message required, bool enumIgnore)
            : base(name, description, required, enumIgnore)
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

            var msg = doValidation(temp);
            if (msg.IsError)
                return msg;

            isDefault = false;
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
                Object.ReferenceEquals(value, null) ? "<null>" : (typeof(T).Name + "[" + value.Length + "]"),
                isDefault ? " (default)" : "");
        }
    }
}
