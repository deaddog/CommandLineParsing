﻿using System;
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
        private Message noValueMessage;
        private Message multipleValuesMessage;

        private List<Func<T, Message>> validators;
        protected Message doValidation(T value)
        {
            for (int i = 0; i < validators.Count; i++)
            {
                var msg = validators[i](value);
                if (msg.IsError)
                    return msg;
            }

            return Message.NoError;
        }

        internal Parameter(string name, string description, Message required)
            : base(name, description, required)
        {
            this.value = default(T);
            this.parser = null;

            this.typeErrorMessage = x => string.Format("Argument \"{0}\" with value \"{1}\" could not be parsed to a value of type {2}.", name, x, typeof(T).Name);
            this.noValueMessage = "No value provided for argument \"" + name + "\".";
            this.multipleValuesMessage = "Only one value can be provided for argument \"" + name + "\".";

            this.validators = new List<Func<T, Message>>();
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
        public Message NoValueMessage
        {
            get { return noValueMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                this.noValueMessage = value;
            }
        }
        public Message MultipleValuesMessage
        {
            get { return multipleValuesMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.IsError)
                    throw new ArgumentException("An error message cannot be the NoError message.", "value");

                this.multipleValuesMessage = value;
            }
        }

        public void Validate(Func<T, Message> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this.validators.Add(validator);
        }

        internal override Message Handle(Argument argument)
        {
            if (parser == null)
                parser = ParserLookup.Table.GetParser<T>();

            T temp;

            if (argument.Count == 0)
                return noValueMessage;
            else if (argument.Count > 1)
                return multipleValuesMessage;
            else if (!parser(argument[0], out temp))
                return typeErrorMessage(argument[0]);

            var msg = doValidation(temp);
            if (msg.IsError)
                return msg;

            value = temp;
            doCallback();

            return Message.NoError;
        }
    }
}
