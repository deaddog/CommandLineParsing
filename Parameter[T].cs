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

        internal Parameter(string name, string description, Message required)
            : base(name, description, required)
        {
            this.value = default(T);
            this.parser = null;
        }

        public virtual T Value
        {
            get { return value; }
        }

        internal override Message Handle(Argument argument)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>();

            return Message.NoError;
        }
    }
}
