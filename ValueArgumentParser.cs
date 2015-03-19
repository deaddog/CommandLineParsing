using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    public abstract class ValueArgumentParser<T, TParser> : ArgumentParser where TParser : ArgumentParser
    {
        private string description;

        private Func<string, Message> typeValidator;
        private List<Func<T, Message>> validator;

        private Message required;

        private Action<T> callback;
        private bool defaultSet;
        private T defaultValue;

        internal Message doTypeValidation(string value)
        {
            return typeValidator(value);
        }
        internal Message doValidationAndCallback(T value)
        {
            for (int i = 0; i < validator.Count; i++)
            {
                var msg = validator[i](value);
                if (msg.IsError)
                    return msg;
            }

            if (callback != null)
                callback(value);

            return Message.NoError;
        }

        internal ValueArgumentParser(string name)
            : base(name)
        {
            this.description = null;

            this.typeValidator = null;
            this.validator = new List<Func<T, Message>>();

            this.required = Message.NoError;

            this.callback = null;
            this.defaultSet = false;
            this.defaultValue = default(T);
        }

        internal override string Description
        {
            get { return description; }
        }

        public TParser WithDescription(string description)
        {
            this.description = description;
            return this as TParser;
        }

        public TParser Callback(Action<T> callback)
        {
            if (this.callback == null)
                this.callback = callback;
            else
                this.callback += callback;

            return this as TParser;
        }

        public TParser DefaultValue(T value)
        {
            if (required.IsError)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.defaultSet = true;
            this.defaultValue = value;

            return this as TParser;
        }

        public TParser ValidateType()
        {
            return ValidateType(x => ("Argument \"" + Name + "\" with value \"" + x + "\" could not be parsed to a value of type " + typeof(T).Name + "."));
        }
        public TParser ValidateType(Message errorMessage)
        {
            return ValidateType(x => errorMessage);
        }
        public TParser ValidateType(Func<string, Message> errorMessage)
        {
            if (this.typeValidator != null)
                throw new InvalidOperationException("Type validation can only be applied to " + this.GetType().Name + " once.");

            this.typeValidator = errorMessage;
            return this as TParser;
        }

        public TParser Validate(Func<T, Message> validator)
        {
            this.validator.Add(validator);

            return this as TParser;
        }
        public TParser Validate(Func<T, bool> validator, Func<T, Message> errorMessage)
        {
            return Validate(x => validator(x) ? Message.NoError : errorMessage(x));
        }
        public TParser Validate(Func<T, bool> validator, Message errorMessage)
        {
            return Validate(x => validator(x) ? Message.NoError : errorMessage);
        }

        internal override bool IsRequired
        {
            get { return !required.IsError; }
        }
        internal override Message RequiredMessage
        {
            get { return required; }
        }

        public TParser Required()
        {
            return Required("You must specify the \"" + Name + "\" argument to execute this command.");
        }
        public TParser Required(Message errorMessage)
        {
            if (defaultSet)
                throw new InvalidOperationException("An argument cannot be required and have a default value at the same time.");

            this.required = errorMessage;

            return this as TParser;
        }
    }
}
