using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public class Parameter<T> : Parameter
    {
        protected T value;
        private TryParse<T> parser;

        private Func<string, Message> typeErrorMessage;

        internal Parameter(string name, string description, Message required)
            : base(name, description, required)
        {
            this.value = default(T);
            this.parser = null;

            this.typeErrorMessage = x => string.Format("Argument \"{0}\" with value \"{1}\" could not be parsed to a value of type {2}.", Name, x, typeof(T).Name);
        }

        public virtual T Value
        {
            get { return value; }
        }

        public Func<string, Message> TypeErrorMessage
        {
            get { return typeErrorMessage; }
            set
            {
                if (typeErrorMessage == null)
                    throw new ArgumentNullException("value");

                typeErrorMessage = value;
            }
        }

        internal override Message Handle(Argument argument)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>();

            T temp;

            if (!parser(argument[0], out temp))
                return typeErrorMessage(argument[0]);

            value = temp;
            doCallback();

            return Message.NoError;
        }
    }
}
